using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenKController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public RosettaWrapper rosettaWrapper;

	public Text msgText;
	public UITextFader msgFader;

	public bool getKey = false;

	public UIDrawHide screenKPanel;

	public string stringBankName;

	// SBName: ScreenK

	const int LAMENTABLE = 0;
	const int ENHORABUEN = 1;
	const int ENHORABUE2 = 2;

	int firstToWinWordGame = -2;

	public void initialize() {
		firstToWinWordGame = -2;
		controllerHub.networkController.sendCommand (controllerHub.gameController.gameState.masterLogin, 
			FGNetworkManager.makeClientCommand ("reportwordgamewin", controllerHub.gameController.gameState.localLogin));
	}

	public void setGetKey(bool k) {
		getKey = k;
	}

	public bool doesPlayerGetKey() {
		return getKey;
	}

	public int getFirstToWinWordGame() {
		return firstToWinWordGame;
	}

	public void resetText() {
		msgFader.Start ();
		msgFader.setOpacity (0.0f);
	}

	public void prepareText() {

		string msg = "";


		int score = 0;
		if (controllerHub.screenFController.wordGameResult == WordGameResult.timeup) {
			msg = rosettaWrapper.rosetta.retrieveString (stringBankName, LAMENTABLE);
			score = 0;
		} else {
			if(controllerHub.gameController.gameState.localNPlayer == firstToWinWordGame) {
				score = 2;
				msg = rosettaWrapper.rosetta.retrieveString (stringBankName, ENHORABUE2);
			}
			else {
				score = 1;
				msg = rosettaWrapper.rosetta.retrieveString (stringBankName, ENHORABUEN);
			}
		}

		msg = msg.Replace("<1>", ("" + score));
		msgText.text = msg;


	}

	// Use this for initialization
	void Start () {

		execute (this, "resetText");
		execute (canvasHub.screenKCanvas, "SetActive", true);
		execute (screenKPanel, "Start");
		waitForTask (screenKPanel, "show");
		execute (canvasHub.screenCCanvas, "SetActive", false);
		execute (canvasHub.screenFCanvas, "SetActive", false);

		execute (this, "initialize"); // ask master who won the word game first
		waitForCondition (-2, "!=", this, "getFirstToWinWordGame"); // wait for master response 
		execute (this, "prepareText");
		delay (0.5f);
		execute (msgFader, "fadeToOpaque");
		execute (this, "setAScreenHidePanel");
		delay (30.0f, this, "checkTouch");
//		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
//		waitForTask (screenKPanel, "hide");
//		execute (canvasHub.screenKCanvas, "SetActive", false);
//		programNotifyFinish ();

		programIf ("GetsKey", "DoesntGetKey", true, "==", this, "doesPlayerGetKey");

		createSubprogram ("GetsKey");
		waitForProgram(controllerHub.screenGController);
		programNotifyFinish ();

		createSubprogram ("DoesntGetKey");
		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		waitForTask (screenKPanel, "hide");
		programNotifyFinish ();


	}

	public void checkTouch() {
		if (Input.GetMouseButtonDown (0)) {
			cancelDelay ();
		}
	}

	public void setAScreenHidePanel() {
		controllerHub.mainGameLoopController.lastScreenDrawHide = screenKPanel;
	}

	// network callbacks
	public void setFirstToWinGame(int pl) {
		firstToWinWordGame = pl;
	}
}
