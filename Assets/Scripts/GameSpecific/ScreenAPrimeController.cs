using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAPrimeController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	// Use this for initialization
	void Start () {
	
		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		execute (canvasHub.screenACanvas, "SetActive", false);
		execute (controllerHub.uiController, "fadeIn");

		createSubprogram ("mirrorTest");
		execute (controllerHub.screenBController, "setLowerBranch");
		waitForProgram (controllerHub.screenBController);
		programNotifyFinish ();

		createSubprogram ("whiteTest");
		waitForProgram (controllerHub.screenDController);
		programNotifyFinish ();

		createSubprogram ("bookTest");
		waitForProgram (controllerHub.screenEController);
		programNotifyFinish ();



		createSubprogram ("finishActivity");
		programNotifyFinish ();


	}

	// network callback
	public void startMirrorTest() {
		goTo ("mirrorTest");
	}

	// network callback
	public void startWhiteTest(string whiteTestHost) {
		//if (!controllerHub.gameController.isMyTurn ()) {
			controllerHub.screenDController.screenDHost = whiteTestHost;
			goTo ("whiteTest");
		//}
	}

	// network callback
	public void startBookTest(string screenEhost) {
		goTo ("bookTest");
	}



	public void finishActivity() {

		goTo ("finishActivity");

	}
}
