using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueGameController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public Text dateTimeText;
	public Text foundPlayersText;

	public int neededPlayers;
	public int reportedPlayers;

	string userLogin;
	string userRoom;

	const float InitialInterval = 1.0f;
	const float Interval = 5.0f;
	float timeToWait;

	public bool tryingToContinue = false;

	bool continueSuccess = false;

	public bool isActive = false;

	public List<string> joinedPlayers;

	public bool doesntHaveNeededPlayers() {
		return (reportedPlayers != (neededPlayers - 1));
	}
	public void checkHasNeededPlayers() {
		updatePlayerCountText ();
		if (reportedPlayers == (neededPlayers - 1))
			cancelDelay ();
	}
	public void updatePlayerCountText() {
		foundPlayersText.text = (reportedPlayers + 1) + "/" + (neededPlayers);
	}

	public void deinitialize() {
		controllerHub.playerSelectController.playerRI.enabled = true;
		controllerHub.playerSelectController.playerRI.gameObject.SetActive (true);
		controllerHub.playerSelectController.playerRI.texture = controllerHub.playerSelectController.players[controllerHub.gameController.gameState.localNPlayer];
		isActive = false;
	}

	public void initialize() {

		controllerHub.saveController.checkQuickSaveInfo ();

		isActive = true;

		continueSuccess = false;

		neededPlayers = controllerHub.gameController.quickSaveInfo.numberOfPlayers;

		reportedPlayers = 0;

		tryingToContinue = true;

		userLogin = controllerHub.gameController.gameState.localLogin;
		userRoom = controllerHub.gameController.quickSaveInfo.roomId;
		controllerHub.gameController.gameState.roomID = userRoom;
		//gameController.randomChallenge = gameController.quickSaveInfo.randomChallenge;
		//gameController.datetimeOfGame = gameController.quickSaveInfo.datetime;


		joinedPlayers = new List<string> ();

		//gameController.network_initGame (userRoom);
		//controllerHub.networkController.initGame();
		controllerHub.networkController.initialize (controllerHub.networkController.bootstrapData.loginServer,
			controllerHub.networkController.bootstrapData.loginServerPort);
		controllerHub.networkController.connectAndEnterRoom ();




		dateTimeText.text = controllerHub.gameController.quickSaveInfo.datetime;

		timeToWait = InitialInterval;

		updatePlayerCountText ();
	}

	public void broadcastContinueBeacon() {
		
		controllerHub.networkController.broadcastUnsafe(FGNetworkManager.makeClientCommand("reportcontinue", userLogin, controllerHub.gameController.quickSaveInfo.randomChallenge, 2));
	}

	// Use this for initialization
	void Start () {

		execute (this, "initialize");
		execute (canvasHub.continueGameCanvas, "SetActive", true);
		execute (controllerHub.uiController, "fadeIn");

		delay (InitialInterval);
		startWhile (this, "doesntHaveNeededPlayers");
			execute(this, "broadcastContinueBeacon");
			delay (Interval, this, "checkHasNeededPlayers");
		endWhile ();
		execute (this, "updatePlayerCountText");

		// mastercontroller.startactivity = "MainGame"
		execute(this, "setContinueSuccess", true);
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (canvasHub.continueGameCanvas, "SetActive", false);
		execute (controllerHub.saveController, "loadQuickSaveData");
		execute (this, "setupRoom");
		execute (this, "deinitialize");
		programNotifyFinish ();

	}

	public void setupRoom() {
		
		for (int i = 0; i < GameController.MaxPlayers; ++i) {
			if (controllerHub.gameController.gameState.playerList [i].isPresent) {
				controllerHub.networkController.receiveSeqFor (controllerHub.gameController.gameState.playerList [i].login);
			}
		}

	}

	public void setContinueSuccess(bool s) {
		continueSuccess = s;
	}


	// received from network, issued by other users
	public void ReportContinue(string otherUser, string randomChallenge, int ttl) {

		if (!isActive)
			return;

		if (randomChallenge.Equals (controllerHub.gameController.quickSaveInfo.randomChallenge)) {

			if (!joinedPlayers.Contains (otherUser)) {
				joinedPlayers.Add (otherUser);
				++reportedPlayers;
			}

		}

		if(ttl > 0)
			controllerHub.networkController.sendCommandUnsafe(otherUser, FGNetworkManager.makeClientCommand("reportcontinue",  userLogin, controllerHub.gameController.quickSaveInfo.randomChallenge, ttl-1));

	}

}
