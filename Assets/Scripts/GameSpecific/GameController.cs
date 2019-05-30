using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class GameState {

	public bool isMaster;
	public string masterLogin;
	public string roomID;
	public string localLogin;
	public string localEMail;
	public string localPasswd;
	public int localNPlayer;
	public int nPlayers;
	public int turnPlayer = 0;

	public int wordGameFirstToWin = -1;

	public List<Player> playerList;

}

[Serializable]
public class LinkKey {

	public int index;
	public DateTime startDate;

	public LinkKey(int i, DateTime d) {
		index = i;
		startDate = d;
	}

}

public class GameController : MonoBehaviour {

	public const int Science = 0;
	public const int Mind = 1;
	public int bookNoPlay = 0;

	public QuickSaveInfo quickSaveInfo;

	public TipSaveData tipSaveData;

	public List<LinkKey>[] obtainedKeys;
	public List<int>[] obtainedKeysIndices;

	public ControllerHub controllerHub;

	public GameState gameState;

	public const int MaxPlayers = 6;

	public const int PlayerNone = -1;

	public void declareBookNoPlay() {
		++bookNoPlay;
		if (bookNoPlay == gameState.nPlayers) {
			// tell self and broadcast to finish book test
			controllerHub.screenHWaitController.finish();
			controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand("hwaitfinish"));
			bookNoPlay = 0;
		}
	}

	public void resetWordGameFirstToWin() {
		gameState.wordGameFirstToWin = -1;
	}
	public void setWordGameFirstToWin(int pl) {
		if (gameState.wordGameFirstToWin == -1) {
			gameState.wordGameFirstToWin = pl;
		}
	}
	public bool isFirstToWinWordGame() {
		return (gameState.wordGameFirstToWin == gameState.localNPlayer);
	}
	// network callback
	public void reportWordGameFirstToWin(string who) {
		controllerHub.networkController.sendCommand (who, 
			FGNetworkManager.makeClientCommand ("firsttowin", gameState.wordGameFirstToWin));
	}

	public bool isMyTurn() {
		return (gameState.localNPlayer == gameState.turnPlayer);
	}

	public void addKey(int type, int index, DateTime startdate) {
		LinkKey key = new LinkKey (index, startdate);
		obtainedKeys [type].Add (key);
		controllerHub.saveController.saveObtainedKeys ();
	}

	public void addSciencePhrase(int i) {
		if (!controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].sciencePhrasesObtained.Contains (i)) {
			controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].sciencePhrasesObtained.Add (i);
			controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].phrasesObtained.Add (new Phrase_t (i, GameController.Science));
		}
	}

	public void addMindPhrase(int i) {
		if (!controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mindPhrasesObtained.Contains (i)) {
			controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mindPhrasesObtained.Add (i);
			controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].phrasesObtained.Add (new Phrase_t (i, GameController.Mind));
		}
	}

	public void addPhrase(int i, bool isScience) {
		if (isScience)
			addSciencePhrase (i);
		else
			addMindPhrase (i);
	}

	// Use this for initialization
	void Start () {
		obtainedKeys = new List<LinkKey>[1];
		obtainedKeys [0] = new List<LinkKey> ();
		//obtainedKeys [Mind] = new List<LinkKey> ();
		obtainedKeysIndices = new List<int> [1];
		obtainedKeysIndices [0] = new List<int> ();
		//obtainedKeysIndices [Mind] = new List<int> ();
		//obtainedKeysIndices[Science].Add (12);
		//obtainedKeysIndices[Mind].Add (1);
		gameState = new GameState ();
		gameState.masterLogin = "";
		gameState.roomID = "";
		gameState.localLogin = "";
		gameState.localPasswd = "";
		gameState.playerList = new List<Player> ();
		for (int i = 0; i < MaxPlayers; ++i) {
			gameState.playerList.Add (new Player ());
		}
	}

	public bool playerPresent(int pl) {
		return gameState.playerList[pl].isPresent;
	}

	// only executed by master
	public void attemptGrabPlayer(int pl, string whoWantsIt) {
		
		if (gameState.playerList [pl].login.Equals ("")) {
			// grant
			//controllerHub.networkController.sendCommand(whoWantsIt, FGNetworkManager.makeClientCommand("takeplayer"));
			controllerHub.networkController.broadcast(FGNetworkManager.makeClientCommand("takeplayer", pl, whoWantsIt));
			gameState.playerList [pl].login = whoWantsIt;
			gameState.playerList [pl].isPresent = true;
			if (!whoWantsIt.Equals (controllerHub.gameController.gameState.localLogin)) {
				controllerHub.playerSelectController.disablePlayer (pl);
			} else {
				//controllerHub.gameController.localNPlayer = pl;
				controllerHub.playerSelectController.takePlayer (pl, whoWantsIt);
			}


		} else {
			// reject
			controllerHub.networkController.sendCommand(whoWantsIt, FGNetworkManager.makeClientCommand("donttakeplayer"));
		}
	}



	public void resetSyncPlayers() {
		controllerHub.networkController.resetSync ();
	}
	public void addSyncPlayers() {
		controllerHub.networkController.addSync ();
	}
	public bool playersAreSynced() {
		
		bool res = false;
		int s;
		s = controllerHub.networkController.getSync ();
		if (s >= gameState.nPlayers) {			
			res = true;
		}
		if (res)
			resetSyncPlayers ();
		return res;
	}


	// Update is called once per frame
	void Update () {
		
	}
}
