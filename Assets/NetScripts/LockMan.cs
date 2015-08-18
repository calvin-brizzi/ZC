using UnityEngine;
using System;
using System.Collections.Generic;

public class LockMan: MonoBehaviour {

	public static readonly int FirstTurnID = 0;
	public static LockMan Instance;

	public int  TurnID = FirstTurnID;
	public int numberOfPlayers = 2;

	private PendingCommands pendingCommands;
	private ConfirmedCommands confirmedCommands;

	private Queue<ICommand> commandsToSend;

	private NetworkView nv;
	private NetMan nm;

	private List<string> playersReady;
	private List<string> confirmedPlayers;

	bool initialized = false;
	

	void Start(){
		enabled = false;
		Instance = this;
		nv = GetComponent<NetworkView> ();
		nm = FindObjectOfType (typeof(NetMan)) as NetMan;

	}

	#region GameStart
	public void InitGameStartLists() {
		if(initialized) { return; }
		
		playersReady = new List<string>(numberOfPlayers);
		confirmedPlayers = new List<string>(numberOfPlayers);
		
		initialized = true;
	}
	public void PrepGameStart() {
		
		Debug.Log ("GameStart called. My PlayerID: " + Network.player.ToString());
		TurnID = FirstTurnID;
		numberOfPlayers = nm.NumberOfPlayers;
		pendingCommands = new PendingCommands(this);
		confirmedCommands = new ConfirmedCommands(this);
		commandsToSend = new Queue<ICommand>();
		
		InitGameStartLists();
		
		nv.RPC ("ReadyToStart", RPCMode.OthersBuffered, Network.player.ToString());
	}
	
	private void CheckGameStart() {
		if(confirmedPlayers == null) {
			Debug.Log("WARNING!!! Unexpected null reference during game start. IsInit? " + initialized);
			return;
		}
		//check if all expected players confirmed our gamestart message
		if(confirmedPlayers.Count == numberOfPlayers - 1) {
			//check if all expected players sent their gamestart message
			if(playersReady.Count == numberOfPlayers - 1) {
				//we are ready to start
				Debug.Log("All players are ready to start. Starting Game.");
				
				//we no longer need these lists
				confirmedPlayers = null;
				playersReady = null;
				
				GameStart ();
			}
		}
	}
	
	private void GameStart() {
		//start the LockStep Turn loop
		//LockStepTurn();
		enabled = true;
	}
	
	[RPC]
	public void ReadyToStart(string playerID) {
		Debug.Log("Player " + playerID + " is ready to start the game.");
		
		//make sure initialization has already happened -incase another player sends game start before we are ready to handle it
		InitGameStartLists();;
		
		playersReady.Add (playerID);
		
		if(Network.isServer) {
			//don't need an rpc call if we are the server
			ConfirmReadyToStartServer(Network.player.ToString() /*confirmingPlayerID*/, playerID /*confirmedPlayerID*/);
		} else {
			nv.RPC("ConfirmReadyToStartServer", RPCMode.Server, Network.player.ToString() /*confirmingPlayerID*/, playerID /*confirmedPlayerID*/);
		}
		
		//Check if we can start the game
		CheckGameStart();
	}
	
	[RPC]
	public void ConfirmReadyToStartServer(string confirmingPlayerID, string confirmedPlayerID) {
		if(!Network.isServer) { return; } //workaround when multiple players running on same machine
		
		Debug.Log("Server Message: Player " + confirmingPlayerID + " is confirming Player " + confirmedPlayerID + " is ready to start the game.");
		
		//validate ID
		if(!nm.players.ContainsKey(confirmingPlayerID)) {
			//TODO: error handling
			Debug.Log("Server Message: WARNING!!! Unrecognized confirming playerID: " + confirmingPlayerID);
			return;
		}
		if(!nm.players.ContainsKey(confirmedPlayerID)) {
			//TODO: error handling
			Debug.Log("Server Message: WARNING!!! Unrecognized confirmed playerID: " + confirmingPlayerID);
		}
		
		//relay message to confirmed client
		if(Network.player.ToString().Equals(confirmedPlayerID)) {
			//don't need an rpc call if we are the server
			ConfirmReadyToStart(confirmedPlayerID, confirmingPlayerID);
		} else {
			nv.RPC ("ConfirmReadyToStart", RPCMode.OthersBuffered, confirmedPlayerID, confirmingPlayerID);
		}
		
	}
	
	[RPC]
	public void ConfirmReadyToStart(string confirmedPlayerID, string confirmingPlayerID) {
		if(!Network.player.ToString().Equals(confirmedPlayerID)) { return; }
		
		//Debug.Log ("Player " + confirmingPlayerID + " confirmed I am ready to start the game.");
		confirmedPlayers.Add (confirmingPlayerID);
		
		//Check if we can start the game
		CheckGameStart();
	}
	#endregion
	
	#region Commands
	public void AddCommand(ICommand action) {
		Debug.Log ("Command Added");
		if(!initialized) {
			Debug.Log("Game has not started, action will be ignored.");
			return;
		}
		commandsToSend.Enqueue(action);
	}
	
	private bool LockStepTurn() {
		Debug.Log ("TurnID: " + TurnID);
		//Check if we can proceed with the next turn
		bool nextTurn = NextTurn();
		if(nextTurn) {
			SendPendingCommand ();
			//the first and second lockstep turn will not be ready to process yet
			if(TurnID >= FirstTurnID + 3) {
				ProcessCommands ();
			}
		}
		//otherwise wait another turn to recieve all input from all players
		
		return nextTurn;
	}
	
	/// <summary>
	/// Check if the conditions are met to proceed to the next turn.
	/// If they are it will make the appropriate updates. Otherwise 
	/// it will return false.
	/// </summary>
	private bool NextTurn() {
		//		Debug.Log ("Next Turn Check: Current Turn - " + TurnID);
		//		Debug.Log ("    priorConfirmedCount - " + confirmedCommands.playersConfirmedPriorCommand.Count);
		//		Debug.Log ("    currentConfirmedCount - " + confirmedCommands.playersConfirmedCurrentCommand.Count);
		//		Debug.Log ("    allPlayerCurrentCommandsCount - " + pendingCommands.CurrentCommands.Count);
		//		Debug.Log ("    allPlayerNextCommandsCount - " + pendingCommands.NextCommands.Count);
		//		Debug.Log ("    allPlayerNextNextCommandsCount - " + pendingCommands.NextNextCommands.Count);
		//		Debug.Log ("    allPlayerNextNextNextCommandsCount - " + pendingCommands.NextNextNextCommands.Count);
		
		if(confirmedCommands.ReadyForNextTurn() && pendingCommands.ReadyForNextTurn()) {
			//increment the turn ID
			TurnID++;
			//move the confirmed commands to next turn
			confirmedCommands.NextTurn();
			//move the pending commands to this turn
			pendingCommands.NextTurn();
			
			return true;
		}
		
		return false;
	}
	
	private void SendPendingCommand() {
		ICommand action = null;
		if(commandsToSend.Count > 0) {
			action = commandsToSend.Dequeue();
		}
		
		//if no action for this turn, send the NoCommand action
		if(action == null) {
			action = new NoCommand();
		}
		//add action to our own list of commands to process
		pendingCommands.AddCommand(action, Convert.ToInt32(Network.player.ToString()), TurnID, TurnID);
		//confirm our own action
		confirmedCommands.playersConfirmedCurrentCommand.Add (Network.player);
		//send action to all other players
		nv.RPC("RecieveCommand", RPCMode.Others, TurnID, Network.player.ToString(), BinarySerialization.SerializeObjectToByteArray(action));
		
		Debug.Log("Sent " + (action.GetType().Name) + " action for turn " + TurnID);
	}
	
	private void ProcessCommands() {
		foreach(ICommand action in pendingCommands.CurrentCommands) {
			action.ProcessCommand();
		}
	}
	
	[RPC]
	public void RecieveCommand(int lockStepTurn, string playerID, byte[] actionAsBytes) {
		//Debug.Log ("Recieved Player " + playerID + "'s action for turn " + lockStepTurn + " on turn " + TurnID);
		ICommand action = BinarySerialization.DeserializeObject<ICommand>(actionAsBytes);
		if(action == null) {
			Debug.Log ("Sending action failed");
			//TODO: Error handle invalid commands recieve
		} else {
			pendingCommands.AddCommand(action, Convert.ToInt32(playerID), TurnID, lockStepTurn);
			
			//send confirmation
			if(Network.isServer) {
				//we don't need an rpc call if we are the server
				ConfirmCommandServer (lockStepTurn, Network.player.ToString(), playerID);
			} else {
				nv.RPC ("ConfirmCommandServer", RPCMode.Server, lockStepTurn, Network.player.ToString(), playerID);
			}
		}
	}
	
	[RPC]
	public void ConfirmCommandServer(int lockStepTurn, string confirmingPlayerID, string confirmedPlayerID) {
		if(!Network.isServer) { return; } //Workaround - if server and client on same machine
		
		//Debug.Log("ConfirmCommandServer called turn:" + lockStepTurn + " playerID:" + confirmingPlayerID);
		//Debug.Log("Sending Confirmation to player " + confirmedPlayerID);
		
		if(Network.player.ToString().Equals(confirmedPlayerID)) {
			//we don't need an RPC call if this is the server
			ConfirmCommand(lockStepTurn, confirmingPlayerID);
		} else {
			nv.RPC("ConfirmCommand", nm.players[confirmedPlayerID], lockStepTurn, confirmingPlayerID);
		}
	}
	
	[RPC]
	public void ConfirmCommand(int lockStepTurn, string confirmingPlayerID) {
		NetworkPlayer player = nm.players[confirmingPlayerID];
		//Debug.Log ("Player " + confirmingPlayerID + " confirmed action for turn " + lockStepTurn + " on turn " + TurnID);
		if(lockStepTurn == TurnID) {
			//if current turn, add to the current Turn Confirmation
			confirmedCommands.playersConfirmedCurrentCommand.Add (player);
		} else if(lockStepTurn == TurnID -1) {
			//if confirmation for prior turn, add to the prior turn confirmation
			confirmedCommands.playersConfirmedPriorCommand.Add (player);
		} else {
			//TODO: Error Handling
			Debug.Log ("WARNING!!!! Unexpected lockstepID Confirmed : " + lockStepTurn + " from player: " + confirmingPlayerID);
		}
	}
	#endregion

	#region Frame
	private int GameFrame = 1;
	private int FramesPerLock = 4;
	private float TimeAcc = 0f;
	private float FrameLength = 0.05f;
	private int GameFramesPerSecond;

	void Awake(){
		GameFramesPerSecond = (int)(1 / FrameLength);
	}
	
	// Update is called once per frame
	void Update () {
		TimeAcc += Time.deltaTime;
		while (TimeAcc > FrameLength) {
			GameFrameTurn();
			TimeAcc -= FrameLength;
		}
	}

	private void GameFrameTurn(){
		if (GameFrame == 0) {
			if (LockStepTurn ()) {
				GameFrame++;
			}
		} else {

			foreach(IUpdatable obj in SceneMan.Man.UpdatableObjects){
				obj.Turn(GameFramesPerSecond);
				if(obj.Finished){
					SceneMan.Man.UpdatableObjects.Remove(obj);
				}
			}

			GameFrame++;
			if(GameFrame == FramesPerLock){
				GameFrame = 0;
			}
		}
	}

	#endregion
}
