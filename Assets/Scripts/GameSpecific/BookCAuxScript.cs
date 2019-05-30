using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookCAuxScript : MonoBehaviour, ButtonPressListener {

	public ControllerHub controllerHub;

	public void buttonPress() {
		controllerHub.screenCController.changeStoryStarting ();
	}
}
