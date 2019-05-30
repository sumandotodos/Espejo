using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAuxScript : MonoBehaviour, ButtonPressListener {

	public ControllerHub controllerHub;

	public void buttonPress() {
		controllerHub.screenIController.changeStoryStarting ();
	}
}
