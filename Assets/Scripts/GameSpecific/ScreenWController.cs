using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenWController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public UIDrawHide panelWDrawHide;

	const int NINGUNA = 0;
	const int UNA = 1;
	const int VARIAS = 2;

	public string textPrefix;

	public RosettaWrapper rosettaWrapper;

	public Text elTexto;
	public UITextFader textoFader;

	int advances;

	public void initializeScreen() {

		textoFader.Start ();
		textoFader.setOpacity (0.0f);
		advances = 3 - controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mirrorDamage;
		controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mirrorDamage = 0;
		if (advances == 0) {
			elTexto.text = rosettaWrapper.rosetta.retrieveString (textPrefix, NINGUNA);
		} else if (advances == 1) {
			elTexto.text = rosettaWrapper.rosetta.retrieveString (textPrefix, UNA);
		} else {
			string txt = rosettaWrapper.rosetta.retrieveString (textPrefix, VARIAS);
			txt = txt.Replace ("<1>", "" + advances);
			elTexto.text = txt;
		}

	}

	// Use this for initialization
	void Start () {

		execute (this, "initializeScreen");
		execute (canvasHub.screenWCanvas, "SetActive", true);
		execute (panelWDrawHide, "Start");
		execute (panelWDrawHide, "reset");
		waitForTask (panelWDrawHide, "show");
		execute (canvasHub.screenACanvas, "SetActive", false);
		delay (0.5f);
		execute (textoFader, "fadeToOpaque");
		delay (5.0f, this, "checkTouch");
		execute (canvasHub.screenBCanvas, "SetActive", true);
		execute (controllerHub.screenBController, "setUpperBranch");
		//waitForTask (panelWDrawHide, "hide");
		//execute (canvasHub.screenWCanvas, "SetActive", false);
		waitForProgram (controllerHub.screenBController);
		programNotifyFinish ();


	}
	
	public void checkTouch() {
		if (Input.GetMouseButtonDown (0))
			cancelDelay ();
	}

}
