using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAController : FGProgram {

	public UITip turnTip;

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;
	public bool block;

	public void startWhiteTestOnOthers() {
		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("startwhitetest", 
			controllerHub.gameController.gameState.localLogin));
	}

	public void initialize()
	{
		block = false;
		if(controllerHub.gameController.tipSaveData.dismissedTips.Contains("starttip1")) {
			turnTip.show(); // manually launch turn tip
		}
	}

	void Start () 
	{		
		execute (canvasHub.screenACanvas, "SetActive", true);
		execute (canvasHub.screenAPrimeCanvas, "SetActive", false);
		execute (this, "initialize");
		execute (controllerHub.uiController, "fadeIn");

		createSubprogram ("mirrorTest");
		execute (controllerHub.networkController, "broadcast", "startmirrortest");
		execute (controllerHub.screenBController, "setLowerBranch");
		waitForProgram (controllerHub.screenBController);
		programNotifyFinish ();

		createSubprogram ("bookTest");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("startbooktest", 
			controllerHub.gameController.gameState.localLogin));
		waitForProgram (controllerHub.screenEController);
		programNotifyFinish ();

		createSubprogram ("whiteTest");
		//execute (controllerHub.masterController, "AppDebug", "WhiteTest!");
		//execute (controllerHub.networkController, "broadcast", "startwhitetest");
		execute(this, "startWhiteTestOnOthers");
		execute (controllerHub.screenBController, "setUpperBranch");
		waitForProgram (controllerHub.screenBController);
		programNotifyFinish ();

		createSubprogram ("blackTest");
		//execute (controllerHub.masterController, "AppDebug", "WhiteTest!");
		//execute (controllerHub.networkController, "broadcast", "startwhitetest");
		execute(this, "startWhiteTestOnOthers");
		execute (controllerHub.screenBController, "setUpperBranch");
		waitForProgram (controllerHub.screenWController);
		programNotifyFinish ();

	}
	
	void Update () {
		update ();
	}

	public void clickOnBook() {
		if (block)
			return;
		goTo ("bookTest");
		block = true;
	}

	public void clickOnMirror() {
		if (block)
			return;
		goTo ("mirrorTest");
		block = true;
	}
		
	public void clickOnWhite() {
		if (block)
			return;
		goTo ("whiteTest");
		block = true;
	}

	public void clickOnBlack() {
		if (block)
			return;
		goTo ("blackTest");
		block = true;
	}


}
