using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishGameController : FGProgram {

	public ControllerHub controllerHub;

	public UIFader finishGameVotationBackgr;
	public UIScaleFader finishGameVotationCircle;

	public Text yesText;

	public int votesForFinishingGame = 0;

	bool voted = false;

	public void resetVotes() {
		voted = false;
		yesText.color = Color.black;
		votesForFinishingGame = 0;
	}

	// Use this for initialization
	void Start () {

		execute (this, "resetVotes");

		createSubprogram ("OpenVotation");
		execute (this, "resetVotes");
		execute (finishGameVotationBackgr, "Start");
		execute (finishGameVotationBackgr, "fadeToOpaque");
		delay (0.5f);
		execute (finishGameVotationCircle, "scaleIn");

		createSubprogram ("CloseVotation");
		execute (finishGameVotationCircle, "scaleOut");
		delay (0.5f);
		execute (finishGameVotationBackgr, "fadeToTransparent");
	}

	// UI callback
	public void gameOverYesButton() {
		if (voted)
			return;
		voted = true;
		yesText.color = Color.blue;
		controllerHub.networkController.sendCommand (
			controllerHub.gameController.gameState.masterLogin,
			FGNetworkManager.makeClientCommand ("voteforfinishgame"));
	}

	public void hideControls() {

		finishGameVotationBackgr.setOpacity (0.0f);
		finishGameVotationCircle.scaleOutImmediately ();

	}

	// UI callback
	public void gameOverNoButton() {

		goTo ("CloseVotation");
		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("cancelfinishgame"));
		controllerHub.screenAController.block = false;

	}

	// network callback
	public void openVotationPanel() {
		goTo ("OpenVotation");
	}

	// network callback
	public void cancelFinishGame() {
		controllerHub.screenAController.block = false;
		goTo ("CloseVotation");
	}

	// network callback. Only the master gets this called
	public void voteForFinishGame() {
		++votesForFinishingGame;
		if (votesForFinishingGame == controllerHub.gameController.gameState.nPlayers) {
			
			controllerHub.mainGameLoopController.finishGame ();
			controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("finishgame"));
		}
	}
}
