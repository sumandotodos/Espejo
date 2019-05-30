using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MasterController : FGProgram {

	public GameObject[] mustBeOn;

	public GameObject blackScreenOfDeath;

	public GameObject upgradeCanvas;
	public UIScaleFader upgradeNoticeScaler;

	const float maxDoubleTapDelay = 0.25f;
	float doubleTapElapsedTime = 0;

	float timer0;
	int state0 = 0;

	[HideInInspector]
	bool showingService = false;

	[HideInInspector]
	bool showingGear = false;

	public UIScaleFader servicePanel;
	public UIFader gearFader;
	public UIDelayFader gearDelay;

	string nextActivity;

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public void showLogoCanvas() {
		canvasHub.logoCanvas.SetActive (true);
	}

	public void hideLogoCanvas() {
		canvasHub.logoCanvas.SetActive (false);
	}

	public void showTitleCanvas() {
		canvasHub.titleCanvas.SetActive (true);
	}

	public void hideTitleCanvas() {
		canvasHub.titleCanvas.SetActive (false);
	}
		
	public void setNextActivity(string newAct) {
		nextActivity = newAct;
	}
	public string getNextActivity() {
		return nextActivity;
	}

	// Use this for initialization
	void Start () {

		foreach(GameObject o in mustBeOn) { o.SetActive (true); }
		FGUtils.bootstrapServers (controllerHub.networkController);

		upgradeCanvas.SetActive (false);
		WWWForm myWWWForm = new WWWForm ();
		myWWWForm.AddField ("app", "EspLite");
		WWW myWWW = new WWW (controllerHub.networkController.bootstrapData.loginServer + "/login/getMinimumBuild", myWWWForm);
		while (!myWWW.isDone) { } // oh, no, don't!!
		if (!myWWW.text.Equals ("")) {
			int minimumBuild;
			int.TryParse (myWWW.text, out minimumBuild);
			if (minimumBuild > FGUtils.build) {
				upgradeCanvas.SetActive (true);
				upgradeNoticeScaler.Start ();
				upgradeNoticeScaler.scaleIn ();
			}
		}

		state0 = 0;

		blackScreenOfDeath.SetActive (false);

		// main program loop

		// initial canvas configuration
		execute (canvasHub.titleCanvas, "SetActive", false);
		execute (canvasHub.choosePlayerScene, "SetActive", false);
		execute (canvasHub.choosePlayerCanvas, "SetActive", false);
		execute (canvasHub.joinNewGameCanvas, "SetActive", false);
		execute (canvasHub.createNewGameCanvas, "SetActive", false);




		// show logo
		execute(canvasHub.logoCanvas, "SetActive", true);
		execute (controllerHub.titleController.mascotScaler, "Start");
		execute (controllerHub.titleController.mascotScaler, "scaleOutImmediately");
		execute (controllerHub.uiController, "fadeIn");
		delay (0.65f);
		execute (controllerHub.titleController.mascotScaler, "scaleIn");
		delay (4.0f, this, "checkCancelDelay");
		debug("Fadeando...");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (this, "hideLogoCanvas");
		programGoTo ("showTitles");




		// show titles
		createSubprogram ("showTitles");
		// show title
		debug("EmpezandoShowTitles...");
		execute(canvasHub.titleCanvas, "SetActive", true);
		waitForProgram (controllerHub.titleController);
		execute (canvasHub.titleCanvas, "SetActive", false);
		programIf("ContinueGame", "", "ContinueGame", "==", this, "getNextActivity");
		programIf ("createNewGame", "joinNewGame", "createNewGame", "==", this, "getNextActivity");




		createSubprogram ("createNewGame");
		// create a new game
		waitForProgram (controllerHub.newGameController);
		programGoTo ("checkStartGame");


		createSubprogram ("joinNewGame");
		// try to join a new game
		waitForProgram (controllerHub.joinNewGameController);
		programGoTo ("checkStartGame");



		createSubprogram ("checkStartGame");
		// check if we have to proceed to main game loop or fall back to titles
		programIf ("SelectPlayer", "Reset", "SelectPlayer", "==", this, "getNextActivity");


		createSubprogram ("Reset");
		// reset all
		loop ();


		createSubprogram ("SelectPlayer");
		// wait until player chooses player
		waitForProgram(controllerHub.playerSelectController);
		waitForProgram (controllerHub.mainGameLoopController);


		createSubprogram ("ContinueGame");
		waitForProgram (controllerHub.continueGameController);
		execute (this, "recoverSession");
		waitForProgram (controllerHub.mainGameLoopController);

		run ();
	
	}

	public void recoverSession() {
		controllerHub.saveController.loadQuickSaveData ();
	}

	public void checkCancelDelay() {
		if (Input.GetMouseButtonDown (0)) {
			cancelDelay ();
		}
	}
	/*
	public void AppDebug(string msg) {
		DebugLog += (msg + "\n");
	}*/
	
	void Update () {
		
		doubleTapElapsedTime += Time.deltaTime;
		if (Input.GetMouseButtonDown (0)) {
			if (doubleTapElapsedTime < maxDoubleTapDelay) {
				showGear ();
			}
			doubleTapElapsedTime = 0.0f;
		}

		update ();	

		if (state0 == 666) {
			timer0 -= Time.deltaTime;
			if (timer0 < 0.0f) {
				controllerHub.networkController.disconnect ();
				state0 = 667;
				timer0 = 0.5f;
			}
		}
		if (state0 == 667) {
			timer0 -= Time.deltaTime;
			if (timer0 < 0.0f) {
				
				SceneManager.LoadScene ("Scenes/ConnectionTest");
				state0 = 0;
			}
		}
	}

	public void showGear() {
		gearDelay.resetTimer ();
		if (!showingGear) {
			showingGear = true;
			gearDelay.resetTimer ();
			gearDelay.going = true;
			gearFader.fadeToOpaque ();
		} else {
			if (!showingService) {
				showingGear = false;
				gearDelay.going = false;
				gearFader.fadeToTransparent ();
			}
		}
	}

	public void toggleServicePanel() {

		if (showingService) {
			showingService = false;
			gearDelay.going = true;
			gearDelay.resetTimer ();
			servicePanel.scaleOut ();
		} else {
			showingService = true;
			gearDelay.going = false;
			servicePanel.scaleIn ();
		}

		//gearDelay.resetTimer ();
		//gearDelay.going = !gearDelay.going;

	}

	public void nuke() {

		//controllerHub.networkController.disconnect ();
		//SceneManager.LoadScene ("Scenes/Loader");


			blackScreenOfDeath.SetActive (true);
			timer0 = 0.25f;
			state0 = 666;


			//yield return loadAll;

		

	}

	public void resetHelp() {
		controllerHub.gameController.tipSaveData.dismissedTips = new List<string> ();
		controllerHub.saveController.saveTipData ();
		toggleServicePanel ();
	}
}
