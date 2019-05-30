using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHButtonAux2 : MonoBehaviour, ButtonPressListener {

	public ControllerHub controllerHub;
	public UIImageToggle toggle;

	public int id;

	public void buttonPress() {
		toggle.toggle ();
	}
}
