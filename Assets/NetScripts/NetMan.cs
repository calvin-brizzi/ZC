using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]

public class NetMan : MonoBehaviour {

	private const string typeName = "ZC";
	private const string gameName = "Test";

	public int NumberOfPlayers = 2;
	public Dictionary<string, NetworkPlayer> players;
	public delegate void NetManEvent ();
	public NetManEvent OnConnectedToGame;
	public NetManEvent OnGameStart;

	private NetworkView nv;

	private void Start() {		
		players = new Dictionary<string, NetworkPlayer>(NumberOfPlayers);
		nv = GetComponent<NetworkView>();
		nv.stateSynchronization = NetworkStateSynchronization.Off;
	}

	private void StartServer()
	{
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		players.Add (Network.player.ToString(), Network.player);

		//MasterServer.ipAddress = "127.0.0.1";
		//MasterServer.RegisterHost(typeName, gameName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			if (GUI.Button(new Rect(100, 250, 250, 100), Network.player.ipAddress))
				Debug.Log("yay");
			if (GUI.Button(new Rect(400, 100, 300, 100), "Connect to server")){
				Network.Connect("127.0.0.1",25000);
				if(OnConnectedToGame != null) {
					OnConnectedToGame();
				}

			}

		}
	}

	private void OnPlayerConnected (NetworkPlayer player) {
		players.Add (player.ToString(), player);
		Debug.Log ("OnPlayerConnected, playerID:" + player.ToString());
		Debug.Log ("Player Count : " + players.Count);
		//Once all expected players have joined, send all clients the list of players
		if(players.Count == NumberOfPlayers) {
			foreach(NetworkPlayer p in players.Values) {
				Debug.Log ("Calling RegisterPlayerAll...");
				nv.RPC("RegisterPlayerAll", RPCMode.Others, p);
			}
			
			//start the game
			nv.RPC ("StartGame", RPCMode.All);
		}
	}

	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
	}

	[RPC]
	public void RegisterPlayerAll(NetworkPlayer player) {
		Debug.Log ("Register Player All called for " + player.ToString());
		players.Add (player.ToString(), player);
	}
	
	[RPC]
	public void StartGame() {
		//send the start of game event
		if(OnGameStart!=null) {
			OnGameStart();
		}
	}


}
