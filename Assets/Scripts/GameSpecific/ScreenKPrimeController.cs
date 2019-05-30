using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenKPrimeController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;
	public RosettaWrapper rosettaWrapper;
	public Text msgText;
	public UITextFader msgFader;
	public int avanzaCasillas;
	public bool getKey = true;
	public string stringBankName;
	public UIBlinker arrowBlinker;

	public UIDrawHide panelKPrime;

	public void setAdvances(int a) {
		avanzaCasillas = a;
	}

	public void setGetKey(bool k) {
		getKey = k;
	}

	public void screenInitialize() {
		arrowBlinker.Start ();
		prepareText ();
	}

	public void prepareText() {

		string msg = "";


		if (avanzaCasillas == 0) {
			msg = rosettaWrapper.rosetta.retrieveString (stringBankName, 0);
		} else {
			if(avanzaCasillas > 1) {
				msg = rosettaWrapper.rosetta.retrieveString (stringBankName, 2);
			}
			else {
				msg = rosettaWrapper.rosetta.retrieveString (stringBankName, 1);
			}
		}

		msg = msg.Replace("<1>", ("" + avanzaCasillas));
		msgText.text = msg;


	}

	public bool doesPlayerGetKey() {
		return getKey;
	}

	void Start () 
	{
		execute (canvasHub.screenKPrimeCanvas, "SetActive", true);
		execute (this, "screenInitialize");
		execute (panelKPrime, "Start");
		waitForTask (panelKPrime, "show");
		delay (0.35f);
		execute (msgFader, "Start");
		execute (msgFader, "fadeToOpaque");
		execute (arrowBlinker, "startBlinking");
		execute (canvasHub.screenDCanvas, "SetActive", false);
		execute (canvasHub.screenHCanvas, "SetActive", false);
		execute (canvasHub.screenAPrimeCanvas, "SetActive", false);
		delay (30.0f, this, "checkTouch");
		programIf ("GetsKey", "DoesntGetKey", true, "==", this, "doesPlayerGetKey");

		createSubprogram ("GetsKey");
		waitForProgram(controllerHub.screenGController);
		programNotifyFinish ();

		createSubprogram ("DoesntGetKey");
		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		waitForTask (panelKPrime, "hide");
		programNotifyFinish ();
	}

	public void checkTouch() {
		if (Input.GetMouseButtonDown (0)) {
			cancelDelay ();
		}
	}

}
