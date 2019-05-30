using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenXController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;
	public Texture[] heroes;
	public RawImage winnerHero;

	public int winHero = -1;

	public void initializeScreen() {

		canvasHub.screenACanvas.SetActive (false);
		canvasHub.screenAPrimeCanvas.SetActive (false);
		controllerHub.finishGameController.hideControls ();
		winnerHero.texture = heroes [winHero];
	
	}

	public bool knowsWinHero() {

		return winHero != -1;

	}

	void Start() {

		waitForCondition (this, "knowsWinHero");
		execute (this, "initializeScreen");
		//debug ("X_entered");
		execute (canvasHub.screenXCanvas, "SetActive", true);
		execute (controllerHub.uiController, "fadeIn");



		delay (20.0f, this, "checkTouch");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (controllerHub.masterController, "nuke");


	}

	public void checkTouch() {
		if (Input.GetMouseButtonDown (0))
			cancelDelay ();
	}
}
