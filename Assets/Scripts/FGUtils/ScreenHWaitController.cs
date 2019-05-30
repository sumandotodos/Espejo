using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHWaitController : FGProgram {

	public ControllerHub controllerHub;

	// Use this for initialization
	void Start () {

		createSubprogram ("Finish");
		programNotifyFinish ();

		createSubprogram ("KPrime");
		waitForProgram (controllerHub.screenKPrimeController);
		programNotifyFinish ();

	}

	// network callback
	public void finish() {
		goTo ("Finish");
	}
	

	// network callback
	public void startKPrime(int advances) {
		
		controllerHub.screenKPrimeController.setGetKey (false);
		controllerHub.screenKPrimeController.setAdvances (advances);
		goTo ("KPrime");
	}

}
