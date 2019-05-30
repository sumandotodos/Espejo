using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public UIDrawHide panelGDrawHide;

	public UIFader keyFader;
	public Texture[] keysTex;
	public RawImage keyRI;
	public UITextFader textFader;

	public int whichKey;
	public int keyIndex;
	int keyType;

	public void setKeyType(int type) {
		keyType = 0; //type;
	}

	public void setKeyColor(int k) { // key color
		whichKey = k;
	}

	public void setKeyIndex(int i) {
		keyIndex = i;
	}

	public void initializeScreen() {
		keyFader.Start ();
		//setKey(Random.Range(0, keysTex.Length));
		keyRI.texture = keysTex [whichKey];
		keyFader.setOpacity (0.0f);
		textFader.Start ();
		controllerHub.gameController.addKey (keyType, keyIndex, System.DateTime.Now);
		//textFader.
	}

	// Use this for initialization
	void Start () {

		execute (this, "initializeScreen");
		execute (canvasHub.screenGCanvas, "SetActive", true);
		execute (panelGDrawHide, "Start");
		waitForTask (panelGDrawHide, "show");
		execute (canvasHub.screenDCanvas, "SetActive", false);
		execute (canvasHub.screenKPrimeCanvas, "SetActive", false);
		execute (canvasHub.screenKCanvas, "SetActive", false);
		delay (0.5f);
		execute (textFader, "fadeToOpaque");
		delay (0.75f);
		execute (keyFader, "fadeToOpaque");
		delay (5.0f, this, "checkTouch");
		execute (canvasHub.screenAPrimeCanvas, "SetActive", true);
		waitForTask (panelGDrawHide, "hide");
		execute (canvasHub.screenGCanvas, "SetActive", false);
		programNotifyFinish ();



	}

	public void checkTouch() {
		if (Input.GetMouseButtonDown (0)) {
			cancelDelay ();
		}
	}
	

}
