using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHController : FGProgram {

	public ControllerHub controllerHub;
	public UITextFader alguienAcerto;
	public UITextFader indicaLosJugadores;
	public UIScaleFader yesButton;
	public UIScaleFader noButton;
	public UIScaleFader readyButton;
	public UICircleDeploy[] player;
	public UIImageToggle[] toggles;

	int minimumPlayersToFinish;

	bool isNoButton = false;

	public void deinitializeScreen() {
		for (int i = 0; i < toggles.Length; ++i) {
			toggles [i].reset ();
		}
	}

	public void initializeScreen() {
		
		alguienAcerto.Start ();
		indicaLosJugadores.Start ();
		yesButton.Start ();
		yesButton.reset ();
		noButton.Start ();
		noButton.reset ();
		readyButton.Start ();
		readyButton.reset ();
		alguienAcerto.GetComponent<Text> ().enabled = false;
		indicaLosJugadores.GetComponent<Text> ().enabled = false;
		for (int i = 0; i < player.Length; ++i) {
			player [i].Start ();
		}
		isNoButton = false;
	}

	public void extendPlayers() {
		
		int nPlayers = controllerHub.gameController.gameState.nPlayers - 1;
		int usedPlayers = 0;
		for (int i = 0; i < GameController.MaxPlayers; ++i) {
			if ((controllerHub.gameController.gameState.playerList [i].isPresent) &&
			   (i != controllerHub.gameController.gameState.localNPlayer)) {
				player [i].setNElements (nPlayers);
				player [i].setIndex (usedPlayers);
				player [i].extend ();
				++usedPlayers;
			}
		}

	}

	void Start () 
	{
		execute (this, "initializeScreen");
		execute (alguienAcerto, "setOpacity", 0.0f);
		execute (indicaLosJugadores, "setOpacity", 0.0f);
		delay (0.5f);
		execute (alguienAcerto, "fadeToOpaque");
		delay (0.25f);
		execute (yesButton, "scaleIn");
		delay (0.15f);
		execute (noButton, "scaleIn");

		createSubprogram ("Yes");
		waitForTask (alguienAcerto, "fadeToTransparent");
		delay (0.25f);
		execute (indicaLosJugadores, "fadeToOpaque");
		execute (readyButton, "scaleIn");
		delay (0.15f);
		execute (this, "extendPlayers");
		delay (1.0f);

		createSubprogram ("No");
		delay (0.2f);
		execute (this, "readyButtonPress");

		createSubprogram ("AllSet");
		debug ("AllsetEnganchado");
		waitForTask (readyButton, "scaleOut");
		execute (this, "deinitializeScreen");
		waitForProgram (controllerHub.screenKPrimeController);
		programNotifyFinish ();
	}

	// UI callback
	public void yesButtonPress() {
		minimumPlayersToFinish = 1;
		goTo ("Yes");
	}

	// UI callback
	public void noButtonPress() {
		minimumPlayersToFinish = 0;
		isNoButton = true;
		goTo ("No");
	}

	// UI callback
	public void readyButtonPress()
	{		
		int numberOfRewardedPlayers = 0;

		for (int i = 0; i < toggles.Length; ++i) {
			if (toggles [i].status ()) {
				++numberOfRewardedPlayers;
			}
		}

		if ((minimumPlayersToFinish == 1) && (numberOfRewardedPlayers == 0))
			return; // must choose at least one player

		for (int i = 0; i < toggles.Length; ++i) {
			if (toggles [i].status ()) {
		
				controllerHub.networkController.sendCommand (
					controllerHub.gameController.gameState.playerList [i].login,
					FGNetworkManager.makeClientCommand ("showkprime", 1));
			} else {
				
					//execute (controllerHub.masterController, "AppDebug", "sending hwaitfinish (hcontrolleR)");
					if (controllerHub.gameController.gameState.playerList [i].isPresent) { // send only to present players
						controllerHub.networkController.sendCommand (
							controllerHub.gameController.gameState.playerList [i].login,
							FGNetworkManager.makeClientCommand ("hwaitfinish"));
					}

			}
		}

		if (numberOfRewardedPlayers >= minimumPlayersToFinish) {
			for (int i = 0; i < player.Length; ++i) {
				player [i].retract ();
			}
			controllerHub.screenKPrimeController.setGetKey (false);
			controllerHub.screenKPrimeController.setAdvances (numberOfRewardedPlayers);
			goTo ("AllSet");
		}
	}
}
