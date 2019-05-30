using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHButtonAux : MonoBehaviour, ButtonPressListener {

	public ControllerHub controllerHub;

	public bool Value;

	public void buttonPress() {
		if (Value) {
			controllerHub.screenHController.yesButtonPress ();
		} else
			controllerHub.screenHController.noButtonPress ();
	}


}
