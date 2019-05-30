using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEController : FGProgram, ButtonPressListener, TimerListener {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;
	public UIDrawHide screenEDrawHide;
	public WellTimer velaTimer;

	public UIAnimatedImage velaAnimatedImage;

	public UISoftBlinker selectorBlinker;

	public Texture velaApagadaTex;

	public RawImage velaIconRI;
	public RawImage[] segments;

	public Color[] playersColors;

	public UIScaleFader velaScaler;
	public UIScaleFader rouletteRigScaler;

	public RouletteController rouletteController;

	bool buttonPressed = false;

	bool timeIsUp = false;

	public void timeup() {
		timeIsUp = true;
		goTo ("DontClickOnVela");
	}

	// spin wheel button
	public void buttonPress() {
		if (buttonPressed)
			return;
		buttonPressed = true;
		goTo ("spin");
	}

	public int numberOfDecisions = 0;
	//public int lastOKPlayer = -1;

	public bool enoughDecisions() {

		bool res = false;
		if (numberOfDecisions == controllerHub.gameController.gameState.nPlayers) {
			res = true;
			numberOfDecisions = 0;
		}
		return res;
	}

	public void incNumberOfDecisions(int pl) {
		if (playingPlayers == null)
			return;
		++numberOfDecisions;
		playingPlayers.Add (pl);
		//lastOKPlayer = pl;
		if (pl != -1) {
			Color col;
			col = playersColors [pl];
			col.a = 1.0f;
			segments [pl].color = col;
		}
		
	}

	public List<int> playingPlayers;

	public void voteVelaYes() {
		incNumberOfDecisions(controllerHub.gameController.gameState.localNPlayer);
		controllerHub.networkController.broadcast(FGNetworkManager.makeClientCommand ("veladecision", controllerHub.gameController.gameState.localNPlayer));
	}

	public void voteVelaNo() {
		incNumberOfDecisions(-1);
		controllerHub.networkController.broadcast(FGNetworkManager.makeClientCommand ("veladecision", -1));

		
	}

	public void declareBookNoPlay() {
		controllerHub.networkController.sendCommand (
			controllerHub.gameController.gameState.masterLogin, 
			FGNetworkManager.makeClientCommand ("declarebooknoplay"));
	}

	public bool immediatelyFinishHWait = false;
	public bool hasToWaitForH() {
		return !immediatelyFinishHWait;
	}

	public void initScreen() {

		//numberOfDecisions = 0;
		finishAngle = -2.0f;
		playingPlayers = new List<int> ();

		buttonPressed = false;

		velaIconRI.gameObject.GetComponent<UIAnimatedImage> ().Start ();
		velaIconRI.gameObject.GetComponent<UIAnimatedImage> ().reset ();
		velaIconRI.texture = velaApagadaTex;

		for (int i = 0; i < GameController.MaxPlayers; ++i) {
			Color col;
			col = playersColors [i];
			col.a = 0.05f;
			segments[i].color = col;
		}
		velaScaler.Start ();
		velaScaler.reset ();
		velaTimer.Start ();
		velaTimer.reset ();
		velaTimer.go ();
		rouletteRigScaler.Start ();
		rouletteRigScaler.reset ();
		screenEDrawHide.Start ();
		rouletteController.initialize ();
		rouletteController.decideAngle ();
		selectorBlinker.Start ();
		selectorBlinker.reset ();

		immediatelyFinishHWait = false;

		timeIsUp = false;

	}

	float finishAngle = -2.0f;

	public void decideAngle() {

		if (controllerHub.gameController.gameState.isMaster) {
			
			finishAngle = rouletteController.decideAngleFromList (playingPlayers);

			if (playingPlayers.Count > 0) {
				controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("escreenangle", finishAngle));
			}
		}
	}

	public bool canExpandRoulette() {
		return (finishAngle > -1.0f);
	}

	public bool proceedToNextScreen() {
		return (rouletteController.selectedItem == controllerHub.gameController.gameState.localNPlayer);
	}

	public void selectorBlinkerGo() {
		selectorBlinker.transform.localRotation = Quaternion.Euler (0, 0, -120.0f - rouletteController.selectedItem * 60.0f);
		selectorBlinker.go ();
	}

	int velaConcedida = 0;

	public bool isVelaConcedida() {
		return velaConcedida != 0;
	}

	public bool velaConcesion() {
		return velaConcedida > 0;
	}

	// Use this for initialization
	void Start () {
	
		execute (this, "initScreen");
		execute (canvasHub.screenECanvas, "SetActive", true);
		execute (screenEDrawHide, "hideImmediately");
		waitForTask (screenEDrawHide, "show");
		execute (canvasHub.screenACanvas, "SetActive", false);

//		createSubprogram ("wantVela");
//		waitForCondition (new FGPMethodCall (this, "isVelaConcedida"));
//		programIf (new FGPCondition (new FGPMethodCall (this, "velaConcesion"), "==", new FGPConstant (true)), "clickOnVela",
//			"DontClickOnVela");
		

		createSubprogram ("clickOnVela");

		delay (1.0f);
		waitForTask (velaScaler, "scaleOut");
		execute (this, "voteVelaYes");
		waitForCondition (this, "enoughDecisions");
		delay (0.09f);
		execute (rouletteRigScaler, "scaleIn");
		execute (this, "decideAngle");

		createSubprogram ("spin");


		waitForCondition (this, "canExpandRoulette");
		waitForTask (rouletteController, "go");
		delay (0.25f);
		execute (this, "selectorBlinkerGo");
//		// sync players
//		execute (controllerHub.uiController, "startWait");
//		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
//		execute (controllerHub.gameController, "addSyncPlayers");
//		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
//		execute (controllerHub.uiController, "endWait");
//		// end sync players
		delay (0.25f);

		programIf ("Plays", "DoesntPlay", true, "==", this, "proceedToNextScreen");

		createSubprogram ("Plays");
		waitForProgram (controllerHub.screenIController);
		programNotifyFinish ();


		createSubprogram ("DoesntPlay");

		delay (0.5f);
		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		waitForTask (screenEDrawHide, "hide");
		execute (canvasHub.screenECanvas, "SetActive", false);
		programIf ("HasToWait", "DontWait", true, "==", this, "hasToWaitForH");
		createSubprogram ("HasToWait");
		waitForProgram (controllerHub.screenHWaitController);
		programNotifyFinish ();
		createSubprogram ("DontWait");
		programNotifyFinish ();


		createSubprogram ("DontClickOnVela");
		execute (this, "voteVelaNo");
		waitForCondition (this, "enoughDecisions");
		execute (this, "decideAngle");
		delay (0.5f);
		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		waitForTask (screenEDrawHide, "hide");
		execute (canvasHub.screenECanvas, "SetActive", false);

		execute(this, "declareBookNoPlay");
		waitForProgram (controllerHub.screenHWaitController);
		programNotifyFinish ();


	}
		

	public void clickOnVela() {
		if (!timeIsUp) {
			velaTimer.stopped = true;
			velaAnimatedImage.go ();
			goTo ("clickOnVela");
		}
	}

	// network callback
	public void setRouletteFinishAngle(float an) {
		finishAngle = an;
		rouletteController.setFinishAngle (an);
	}
}
