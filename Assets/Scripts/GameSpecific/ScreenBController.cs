using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBController : FGProgram, ButtonPressListener {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public UIDrawHide screenBPanel;

	public RouletteController rouletteController;

	public UISoftBlinker selectorBlinker;

	public Animator turnScreenAnimator;

	public int branch;

	bool spinButtonPressed_lock = false;

	public void setUpperBranch() {
		branch = 1;
	}

	public void setLowerBranch() {
		branch = 0;
	}

	public bool isUpperBranch() {
		return branch == 1;
	}

	// spin wheel button
	public void buttonPress() {
		if (spinButtonPressed_lock == true)
			return;
		spinButtonPressed_lock = true;
		goTo ("spin");
	}

	public void arrowsGo() {
		turnScreenAnimator.SetTrigger ("Unturn");
	}

	public void screenInitialization() {
		spinButtonPressed_lock = false;
		selectorBlinker.Start ();
		selectorBlinker.reset ();
	}
		
	public void selectorBlinkerGo() {
		selectorBlinker.transform.localRotation = Quaternion.Euler (0, 0, -120.0f - rouletteController.selectedItem * 60.0f);
		selectorBlinker.go ();
	}

	public void prepareDScreen() {
		controllerHub.screenDController.setScienceQuestion (rouletteController.selectedTestType == 0);
	}

	public void prepareFScreen() {
		controllerHub.screenFController.isScienceCloze = (rouletteController.selectedTestType == 0);
	}

	public void broadcastSelectedTestType() {
		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("updatelasttype", rouletteController.selectedTestType));
	}

	// Use this for initialization
	void Start () {

		execute (this, "screenInitialization");
		execute (rouletteController, "initialize");
		execute (rouletteController, "decideAngle");
		execute (canvasHub.screenBCanvas, "SetActive", true);
		execute (screenBPanel, "Start");
		execute (screenBPanel, "hideImmediately");
		waitForTask (screenBPanel, "show");
		execute (canvasHub.screenACanvas, "SetActive", false);
		execute (canvasHub.screenAPrimeCanvas, "SetActive", false);
		execute (canvasHub.screenWCanvas, "SetActive", false);


		createSubprogram ("spin");
		waitForTask (rouletteController, "go");
		execute (this, "broadcastSelectedTestType");
		//execute (controllerHub.masterController, "AppDebug", "ROUL FINISH");
		delay (0.25f);
		execute (this, "selectorBlinkerGo");
		//execute (controllerHub.masterController, "AppDebug", "BLINK!");
		delay (1.0f);
		//execute (controllerHub.masterController, "AppDebug", "FINISH BLINK");


		programIf ("UpperBranch", "LowerBranch", true, "==", this, "isUpperBranch");

		createSubprogram ("UpperBranch");
		//execute (controllerHub.masterController, "AppDebug", "UPPER BRANCH");
		execute (this, "prepareDScreen");
		waitForProgram (controllerHub.screenDController);
		programNotifyFinish ();

		createSubprogram ("LowerBranch");
		//execute (controllerHub.masterController, "AppDebug", "LOWER BRANCH");
		delay (0.25f);
		execute (this, "arrowsGo");
		//execute (controllerHub.masterController, "AppDebug", "ARROWS GO!");
		delay (3.0f);
		execute (canvasHub.screenFCanvas, "SetActive", true);
		//execute (controllerHub.masterController, "AppDebug", "ACTIVE F CANVAs");
		//waitForTask (screenBPanel, "hide");
		//execute (canvasHub.screenBCanvas, "SetActive", false);
		execute (this, "prepareFScreen");
		waitForProgram (controllerHub.screenFController);
		programNotifyFinish ();

	}
	
	// Update is called once per frame
	void Update () {
		update ();
	}


}
