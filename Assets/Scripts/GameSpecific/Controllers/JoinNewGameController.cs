using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JoinNewGameController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public QRCodeDecodeController qrDecoder;

	public Text numberOfPlayersText;

	public AudioClip gong_N;

	int state;

	bool firstSetNPlayers = true;

	public FGTable messagesTable;
	public UIScaleFader updateNoticeScaler;
	public Text updateNoticeText;
	public MasterController masterController;

	public void stop() {
		state = 0;
	}

	public void qrDecodeReady(string payload) {

		string myUser;

		Handheld.Vibrate ();

		myUser = controllerHub.gameController.gameState.localLogin.ToLower ();

		string[] arg = payload.Split (':');

		controllerHub.gameController.gameState.roomID = arg [1];

		if (!arg [1].StartsWith (FGUtils.localGamePrefix))
			return;

//		if (!arg [3].Equals (FGUtils.compatibilityCode))
//			return;

//		if (arg [0].ToLower ().Equals (myUser)) {
//			showRepeatedUser (myUser);
//			myUser = myUser.Replace ("@", "_");
//		}
		controllerHub.gameController.gameState.masterLogin = arg[0];

		//if (arg [2].Equals ("newgame")) {
			
			//controllerHub.networkController.initGame_withUser (myUser);
			//controllerHub.networkController.sendCommand (arg [0], "playerready:" + controllerHub.gameController.gameState.localLogin + ":" + FGUtils.compatibilityCode);
			controllerHub.networkController.initialize (controllerHub.networkController.bootstrapData.socketServer,
				controllerHub.networkController.bootstrapData.socketServerPort);
			controllerHub.networkController.initGame_withRoom (arg [1]);
			controllerHub.audioController.playSound (gong_N);
			goTo ("WaitForRoomUUID");

		//} 

		qrDecoder.stopWebcam ();


	}

	public void stopJoinNewGame() {
		qrDecoder.stopWebcam ();
	}

	public void initializeJoinNewGame() {
		qrDecoder.initialize ();
		qrDecoder.startWebcam ();
		qrDecoder.onQRScanFinished += qrDecodeReady;
		controllerHub.gameController.gameState.isMaster = false;
		updateNoticeScaler.Start ();
		updateNoticeScaler.scaleOutImmediately ();
		updateNoticeText.text = (string)messagesTable.getElement (0, FGUtils.MsgIncompatVersion);
		numberOfPlayersText.text = "1";
	}

	public void communicateToMaster() {
		Debug.Log ("Communicating to master: " + controllerHub.gameController.gameState.masterLogin);
		controllerHub.networkController.sendCommand (controllerHub.gameController.gameState.masterLogin, "playerready:" + controllerHub.gameController.gameState.localLogin + ":" + FGUtils.compatibilityCode);
	}

	public void Start() {

		execute (this, "initializeJoinNewGame");
		execute (this.canvasHub.joinNewGameCanvas, "SetActive", true);
		execute (controllerHub.uiController, "fadeIn");


		createSubprogram ("WaitForRoomUUID");
		debug ("Waiting...");
		debug ("Waiting...");
		waitForCondition (controllerHub.networkController, "localUserLoginSet");
		execute(controllerHub.networkController, "enterRoom");
		execute (this, "communicateToMaster");


		createSubprogram ("cancel");

		execute (controllerHub.masterController, "setNextActivity", "Reset");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (this, "stopJoinNewGame");
		programNotifyFinish ();



		createSubprogram ("startGame");

		execute (controllerHub.masterController, "setNextActivity", "SelectPlayer");
		waitForTask(controllerHub.uiController, "fadeOutTask", this);
		execute (this, "stopJoinNewGame");
		execute (canvasHub.joinNewGameCanvas, "SetActive", false);
		programNotifyFinish ();



	}


	public void showCompatibility() {
		updateNoticeText.text = (string)messagesTable.getElement (0, FGUtils.MsgIncompatVersion);
		updateNoticeText.text = updateNoticeText.text.Replace ("\\n", "\n");
		updateNoticeScaler.scaleIn ();
	}

	public void showRepeatedUser(string offendingUser) {
		updateNoticeText.text = (string)messagesTable.getElement (0, FGUtils.MsgRepeatedLogin);
		updateNoticeText.text = updateNoticeText.text.Replace ("\\n", "\n");
		updateNoticeText.text = updateNoticeText.text.Replace ("<1>", offendingUser);
		updateNoticeScaler.scaleIn ();
	}


//	// called by network command setnplayers
//	//  gameController.localPlayerN is set here
	public void setNPlayers(int pl) {

		controllerHub.gameController.gameState.nPlayers = pl;
		numberOfPlayersText.text = "" + pl;
//		if (firstSetNPlayers) {
//			// acquire player number
//			gameController.localPlayerN = pl - 1;
//			firstSetNPlayers = false;
//		}

	}


	void Update() {
		update ();
	}

	// UI Events callbacks
	public void backArrow() 
	{
		masterController.nuke ();

	}
		

	// network callbacks
	public void startGame(string login, string randomChallenge, string networkDateTime) {
		controllerHub.gameController.quickSaveInfo.randomChallenge = randomChallenge;
		controllerHub.gameController.quickSaveInfo.datetime = networkDateTime.Replace ("_", " ");
		controllerHub.gameController.quickSaveInfo.datetime = controllerHub.gameController.quickSaveInfo.datetime.Replace ("!", ":");
		controllerHub.gameController.gameState.masterLogin = login;

		//controllerHub.networkController.sendMessage ("play Esp"); // do not consume credits

		goTo ("startGame");
	}


}
