using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NewGameActivityController : FGProgram {

	public QRCodeEncodeController qrEncoder;
	public RawImage resultImage;

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public Button startGameButton;

	public Text numberOfPlayersText;
    public Text playersRemainingText;

	List<string> seenPlayers;

	public FGTable messagesTable;
	public UIScaleFader updateNoticeScaler;
	public Text updateNoticeText;
	public MasterController masterController;




	int encoderTurn = 0;

	int state = 0;
	int nPlayers = 1;

	string roomID;

	WWW www;
	WWWForm wwwForm;

	public void initializeNewGame() {
		qrEncoder.initialize ();
		qrEncoder.onQREncodeFinished += qrEncodeReady;
		startGameButton.interactable = false;
        startGameButton.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.25f);
        startGameButton.GetComponentInChildren<FGText>().color = new Color(1, 1, 1, 0.25f);
        controllerHub.gameController.gameState.isMaster = true;
		controllerHub.gameController.gameState.masterLogin = controllerHub.gameController.gameState.localLogin;
		//controllerHub.masterController.AppDebug ("Newing...");
		//controllerHub.masterController.AppDebug ("Master is : " + controllerHub.gameController.gameState.localLogin);
		nPlayers = 1;
		updateNoticeScaler.Start ();
		updateNoticeScaler.scaleOutImmediately ();
		updateNoticeText.text = (string)messagesTable.getElement (0, FGUtils.MsgIncompatVersion);
		numberOfPlayersText.text = "1";
        playersRemainingText.text = "Faltan 2 más";
		seenPlayers = new List<string> ();

	}


	public void showCompatibility() {
		updateNoticeText.text = (string)messagesTable.getElement (0, FGUtils.MsgIncompatVersion);
		updateNoticeText.text = updateNoticeText.text.Replace ("\\n", "\n");
		updateNoticeScaler.scaleIn ();
	}

	public void showRepeatedUser(string offendingUser) {
		updateNoticeText.text = (string)messagesTable.getElement (0, FGUtils.MsgRepeatedLogin);
		updateNoticeText.text = updateNoticeText.text.Replace ("\\n", "\n");
		//updateNoticeText.text = updateNoticeText.text.Replace ("<1>", offendingUser);
		updateNoticeScaler.scaleIn ();
	}

	public void storeNewRoomID() {

		roomID = FGUtils.localGamePrefix + controllerHub.networkController.wwwResult ();
		controllerHub.gameController.gameState.roomID = roomID;

	}

	public void storeRandom() {
		
		controllerHub.gameController.quickSaveInfo.randomChallenge = FGUtils.localGamePrefix + Random.Range (0, System.Int32.MaxValue ).ToString ();

	}

	public void storeDateTime() {
		
		controllerHub.gameController.quickSaveInfo.datetime = System.DateTime.Now.ToString();

	}

	public void prepareQRCode() {

		string qrContents = controllerHub.networkController.localUserLogin + ":" + roomID;
		qrEncoder.Encode (qrContents + ":");

	}

	public void broadcastStartGame() {
		string networkDateTime = controllerHub.gameController.quickSaveInfo.datetime.Replace (" ", "_"); // no spaces
		networkDateTime = networkDateTime.Replace (":", "!"); // or colons, please
		controllerHub.networkController.broadcast ( FGNetworkManager.makeClientCommand("startgame", controllerHub.gameController.gameState.localLogin,
			controllerHub.gameController.quickSaveInfo.randomChallenge, networkDateTime));
		string playersString = "";
		for (int i = 0; i < seenPlayers.Count; ++i) {

			playersString += (seenPlayers[i] + ":");

		}
		playersString += "null:";
		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand("roomplayers:", playersString));
		//controllerHub.masterController.AppDebug("Sent command " + FGNetworkManager.makeClientCommand("startgame", controllerHub.gameController.gameState.localLogin));

		//controllerHub.networkController.sendMessage ("play Esp"); do not consume credits, plees!
	}

	public void copyUserLoginToMaster() {
		controllerHub.gameController.gameState.masterLogin = controllerHub.networkController.localUserLogin;
	}

	// Use this for initialization
	void Start () {

		//debug ("NEWGAMECONTROLLER_STARTED");
		execute (this, "initializeNewGame");
		execute (canvasHub.createNewGameCanvas, "SetActive", true);
		execute (controllerHub.uiController, "fadeIn");
		execute (controllerHub.networkController, "getFreshRoomID");
		waitForCondition(true, "==", controllerHub.networkController, "httpRequestIsDone");
		//debug ("NEW_game_HTTP_request_for_fresh_id_is_over");
		//debug (controllerHub.networkController, "wwwResult");
		execute (this, "storeNewRoomID");
		execute (this, "storeRandom");
		execute (this, "storeDateTime");
		execute (controllerHub.networkController, "initGame");
		waitForCondition(controllerHub.networkController, "localUserLoginSet");
		//debug ("Returned_results_"); 
		execute (this, "copyUserLoginToMaster");
		execute (this, "prepareQRCode");
		//execute (controllerHub.networkController, "initGame_void");
		execute(controllerHub.networkController, "enterRoom");


		createSubprogram ("cancel");
		execute (controllerHub.masterController, "setNextActivity", "Reset");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		programNotifyFinish ();


		createSubprogram ("startGame");
		execute (this, "broadcastStartGame");
		execute (controllerHub.masterController, "setNextActivity", "SelectPlayer");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (canvasHub.createNewGameCanvas, "SetActive", false);
		programNotifyFinish ();		
	}

	public void qrEncodeReady(Texture encodedQR) {

			resultImage.texture = encodedQR;

	}
		
		

	public void addPlayer(string playerId, string compatCode) {

		playerId = playerId.ToLower ();

//		if (playerId.Equals (controllerHub.gameController.gameState.localLogin) || seenPlayers.Contains (playerId)) {
//			showRepeatedUser (playerId);
//			controllerHub.networkController.broadcast ("showrepeated:" + playerId + ":");
//			return; // do not allow repeated logins
//		}

		int myCompat, playerCompat;
		int.TryParse (FGUtils.compatibilityCode, out myCompat);
		int.TryParse (compatCode, out playerCompat);
//		if (myCompat != playerCompat) {
//			string neededCompat = FGUtils.compatibilityCode;
//			if (playerCompat > myCompat)
//				neededCompat = compatCode;
//			showCompatibility ();
//			controllerHub.networkController.broadcast ("showcompat:" + neededCompat + ":");
//			return;
//
//		}

		if (nPlayers == GameController.MaxPlayers) {
			controllerHub.networkController.sendCommandUnsafe (playerId, "nuke:$");
			controllerHub.networkController.unseeOrigin (playerId);
			return;
		}

		++nPlayers;
		seenPlayers.Add (playerId);
		controllerHub.gameController.gameState.nPlayers = nPlayers;
		controllerHub.networkController.broadcast ("setnplayers:" + nPlayers+":");
		numberOfPlayersText.text = "" + nPlayers;
        if(nPlayers == 2)
        {
            playersRemainingText.text = "Falta 1 más";
        }
        else
        {
            playersRemainingText.text = "";
        }
        if (nPlayers >= 3) {
			startGameButton.interactable = true;
            startGameButton.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            startGameButton.GetComponentInChildren<FGText>().color = new Color(1, 1, 1, 1);
        }

	}
	
	// Update is called once per frame
	void Update () {

		update ();

	}


	public void backButton() {
		masterController.nuke ();
	}


	public void startGameButtonPress() {
		goTo ("startGame");
	}

	/*
	 * 
	 * 
	 * Network Callbacks
	 * 
	 */
//	public void startGameCallback() {
//		//fader.fadeOutTask (this);
//		//gameController.network_broadcast ("startscan");
//		//gameController.setStartActivity ("ScanPlayers");
//		//state = 4;
//		goTo("startGame");
//		
//	}


	// network callbacks
	public void addPlayer() {
		++nPlayers;
		controllerHub.gameController.gameState.nPlayers = nPlayers;
		controllerHub.networkController.broadcast ("setnplayers:" + nPlayers+":");
		numberOfPlayersText.text = "" + nPlayers;
		startGameButton.interactable = true;
	}
}
