using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public TextureToggle[] peanas;
	public TextureToggle[] reflejos;

	public Texture[] players;
	public RawImage playerRI;

	int thePlayerIWant;

	public void assignPlayerToServicePanel() {
		playerRI.texture = players [thePlayerIWant];
		playerRI.enabled = true;
	}

	public void sendGrabPlayerCommand() {
		controllerHub.networkController.sendCommand (controllerHub.gameController.gameState.masterLogin,
			FGNetworkManager.makeClientCommand ("grabplayer", thePlayerIWant, controllerHub.gameController.gameState.localLogin));
	}

	// Use this for initialization
	void Start () {

		execute (canvasHub.choosePlayerScene, "SetActive", true);
		execute (canvasHub.choosePlayerCanvas, "SetActive", true);
		execute (controllerHub.uiController, "fadeIn");


		// when I press the OK button...
		createSubprogram ("attemptGrabPlayer");
		// tell the master I want that player, and wait for response
		execute (this, "sendGrabPlayerCommand");



		createSubprogram ("playerGranted");

		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (canvasHub.choosePlayerScene, "SetActive", false);
		execute (canvasHub.choosePlayerCanvas, "SetActive", false);
		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		execute (this, "assignPlayerToServicePanel");
		programNotifyFinish (); // back to mastercontroller


		createSubprogram ("allPlayersReady");
		programNotifyFinish ();


	}
		

	public void doSomething() {

	}
	
	// Update is called once per frame
	void Update () {
		update ();
	}

	// UI callbacks
	public void buttonPress(int pl) {

		thePlayerIWant = pl;
		goTo ("attemptGrabPlayer");

	}



	// network callbacks
	public void takePlayer(int pl, string who) {
		
		controllerHub.gameController.gameState.playerList [pl].login = who;
		controllerHub.gameController.gameState.playerList [pl].isPresent = true;

		if (controllerHub.gameController.gameState.localLogin.Equals (who)) {
			controllerHub.gameController.gameState.localNPlayer = pl;
			goTo ("playerGranted");
		} else {
			disablePlayer (pl);
		}
	}

	public void dontTakePlayer() {
		disablePlayer (thePlayerIWant);
	}

	public void disablePlayer(int pl) {
		peanas [pl].toggleTexture ();
		reflejos [pl].toggleTexture ();
	}

	public void allPlayersReady() {
		goTo ("allPlayersReady");
	}

}
