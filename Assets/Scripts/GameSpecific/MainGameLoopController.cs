using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameLoopController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public UIDrawHide lastScreenDrawHide;

	public UIDrawHide[] drawHides;
	public GameObject[] thingsToDisable;


	public void chooseNextPlayer() {


		while (!controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.turnPlayer].isPresent) {
			controllerHub.gameController.gameState.turnPlayer = (controllerHub.gameController.gameState.turnPlayer+1) % GameController.MaxPlayers;
		}
			

	}

	public void nextTurn() {
		controllerHub.gameController.gameState.turnPlayer = (controllerHub.gameController.gameState.turnPlayer+1) % GameController.MaxPlayers;
		controllerHub.gameController.gameState.wordGameFirstToWin = -1;
		controllerHub.gameController.bookNoPlay = 0;
		controllerHub.saveController.saveQuickSaveInfo ();
		controllerHub.saveController.saveQuickSaveData ();
		controllerHub.saveController.saveTipData ();

	}

	public bool isMyTurn() {
		return (controllerHub.gameController.gameState.localNPlayer == controllerHub.gameController.gameState.turnPlayer);
	}

	public void resetAllDrawHides() {
		for (int i = 0; i < drawHides.Length; ++i) {
			drawHides [i].Start ();
			drawHides [i].hide ();

			//drawHides [i].reset ();
		}
		for (int i = 0; i < thingsToDisable.Length; ++i) {
			thingsToDisable [i].SetActive (false);

			//drawHides [i].reset ();
		}
	}

	// Use this for initialization
	void Start () {

		// main game loop

		execute (this, "resetAllDrawHides");
		execute (this, "chooseNextPlayer");
		programIf ("myTurn", "notMyTurn", true, "==", this, "isMyTurn");


		//
		createSubprogram ("myTurn");
		execute (canvasHub.screenAPrimeCanvas, "SetActive", false);
		waitForProgram (controllerHub.screenAController);
		programGoTo ("syncing");



		//
		createSubprogram ("notMyTurn");
		waitForProgram (controllerHub.screenAPrimeController);
		programGoTo ("syncing");

		//
		createSubprogram ("syncing");

		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		execute (this, "nextTurn");
		loop ();


		//
		createSubprogram ("finishGame");

		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		waitForProgram (controllerHub.screenXController); // won't be returning from this, I'm afraid... :)


	}

	public void finishGame() {		
		controllerHub.saveController.resetQuickSaveInfo();
		cancelWaitForProgram ();
		goTo ("finishGame");
	}
	

}
