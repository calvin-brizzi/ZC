using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public Canvas joinMenu;
    public Canvas serverMenu;
    public Button join;
    public Button server;
    public Button exit;
    public Text ip;
    public InputField serverIP;

 

	private void Start() {		
		players = new Dictionary<string, NetworkPlayer>(NumberOfPlayers);
		nv = GetComponent<NetworkView>();
		nv.stateSynchronization = NetworkStateSynchronization.Off;
        joinMenu = joinMenu.GetComponent<Canvas>();
        serverMenu = serverMenu.GetComponent<Canvas>();
        serverIP = serverIP.GetComponent<InputField>();

        join = join.GetComponent<Button>();
        server = server.GetComponent<Button>();

        joinMenu.enabled = false;
        serverMenu.enabled = false;
        ip = ip.GetComponent<Text>();
	}

    public void serverPressed()
    {
        serverMenu.enabled = true;
        join.enabled = false;
        server.enabled = false;
        StartServer();
        ip.text = ip.text + "\n" + Network.player.ipAddress;
    }

    public void joinPressed()
    {
        joinMenu.enabled = true;
        join.enabled = false;
        server.enabled = false;
    }

    public void ipEntered() {
        string sip = serverIP.text;
        Debug.Log(sip);
        Network.Connect(sip, 25000);
    }

    public void exitGame() {
        Application.Quit();
    }

	private void StartServer()
	{
		Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
        VarMan.Instance.pNum = 1;
		players.Add (Network.player.ToString(), Network.player);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
        //VarMan.Instance.pNum = 1;
	}

	void OnGUII()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			if (GUI.Button(new Rect(100, 250, 250, 100), Network.player.ipAddress))
				Debug.Log("yay");
			if (GUI.Button(new Rect(400, 100, 300, 100), "Connect to server")){
				Network.Connect("10.0.0.9",25000);
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
        VarMan.Instance.pNum = 2;
	}

	[RPC]
	public void RegisterPlayerAll(NetworkPlayer player) {
		Debug.Log ("Register Player All called for " + player.ToString());
		players.Add (player.ToString(), player);
	}
	
	[RPC]
	public void StartGame() {
        Application.LoadLevel(1);
		//send the start of game event
		if(OnGameStart!=null) {
			OnGameStart();
		}
	}


}
