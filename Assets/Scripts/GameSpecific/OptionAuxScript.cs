using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionAuxScript : MonoBehaviour, ButtonPressListener {

	public ControllerHub controllerHub;

	public int option;

	public void buttonPress() {

		controllerHub.screenDController.clickOnOption (option);

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
