using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenZController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public UIDrawHide panelZDrawHide;
	public UIDrawHide panelHDrawHide;

	public float timeSeconds = 120.0f;
	public float reserveSeconds = 60.0f;

	public AudioClip alarmSound_N;

	bool canPushButton = false;

	public void initScreen() {
		canPushButton = false;
	}

	public void enablePushButton() {
		canPushButton = true;
	}

	public void vibrate() {
		Handheld.Vibrate ();
	}

	// Use this for initialization
	void Start () {

		execute (this, "initScreen");
		execute (canvasHub.screenZCanvas, "SetActive", true);
		execute (panelZDrawHide, "Start");
		execute (panelZDrawHide, "hideImmediately");
		waitForTask (panelZDrawHide, "show");
		execute (this, "enablePushButton");
		execute (canvasHub.screenECanvas, "SetActive", false);
		execute (canvasHub.screenICanvas, "SetActive", false);
		delay (timeSeconds);
		execute (controllerHub.audioController, "playSound", alarmSound_N);
		execute (this, "vibrate");
		delay (reserveSeconds);
		programGoTo ("finishing");

		createSubprogram ("finishing");
//		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		execute(canvasHub.screenHCanvas, "SetActive", true);
		delay (0.1f);
		execute (panelHDrawHide, "Start");
		execute (panelHDrawHide, "showImmediately");
		execute (controllerHub.screenHController, "initializeScreen");
		waitForTask (panelZDrawHide, "hide");
		waitForProgram (controllerHub.screenHController);
		programNotifyFinish();

	}

//	public void checkFinish() {
//		goTo ("checkfinish");
//	}

	public void finishButtonPress() {
		if (canPushButton) {
			cancelDelay ();
			goTo ("finishing");
			canPushButton = false;
		}
	}


}
