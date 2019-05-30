using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public GameObject linkPrefab;
	public GameObject linksParent;

	public const int NColors = 6;

	//public StringBank[] URLSource;
	public FGTable linkDB;

	//GameObject disposableContainer;

	public void cleanUp() {

		Transform[] ts = linksParent.GetComponentsInChildren<Transform>();
		for (int i = 0; i < ts.Length; ++i) {
			if (ts [i].gameObject != linksParent) {
				Destroy (ts [i].gameObject);
			}
		}

	}

	public void buildLinkArray() {

		controllerHub.saveController.loadObtainedKeys ();

		//for (int k = 0; k < 1; ++k) {
			for (int i = 0; i < controllerHub.gameController.obtainedKeys [0].Count; ++i) {
				int index = controllerHub.gameController.obtainedKeys [0] [i].index;
				System.DateTime time = controllerHub.gameController.obtainedKeys [0] [i].startDate;
				System.TimeSpan deltaTime = System.DateTime.Now - time;
				if (deltaTime.Days <= 7) {
					controllerHub.gameController.obtainedKeysIndices [0].Add (index);
				}
			}
		//}

		for (int k = 0; k < linkDB.nRows(); ++k) {

			//for (int i = 0; i < linkDB[k].phrase.Length; ++i) {
				GameObject newLinkGO = (GameObject)Instantiate (linkPrefab);
				newLinkGO.transform.SetParent (linksParent.transform); //.SetParent (linksParent.transform);
				UITargetAnimation targetAnim = newLinkGO.GetComponent<UITargetAnimation> ();
				//targetAnim.setFrame (URLSource[k].color [i]);
			int col = FGUtils.pseudoRandom(k);
			col = col % NColors;
				targetAnim.setFrame(col);
				UILink uiLink = newLinkGO.GetComponent<UILink> ();
			string pureURL = (string)linkDB.getElement (1, k);
			string fixedURL = pureURL.Replace("\n", "");
			fixedURL = pureURL.Replace ("\r", "");
			uiLink.URL = fixedURL;
				UIButtonPress buttonPress = newLinkGO.GetComponent<UIButtonPress> ();
				buttonPress.controllerHub = controllerHub;

				if (controllerHub.gameController.obtainedKeysIndices[0].Contains (k)) {
					newLinkGO.GetComponent<RawImage> ().color = new Color (1, 1, 1, 1);
					newLinkGO.GetComponent<RawImage> ().raycastTarget = true;
					buttonPress.enabled = true;
				} else {
					newLinkGO.GetComponent<RawImage> ().color = new Color (1, 1, 1, 0.25f);
					newLinkGO.GetComponent<RawImage> ().raycastTarget = true;
					buttonPress.enabled = false;
				}

			//}

		}

		for (int k = 0; k < 4; ++k) {


			GameObject newLinkGO = (GameObject)Instantiate (linkPrefab);
			newLinkGO.transform.SetParent (linksParent.transform); 
			UITargetAnimation targetAnim = newLinkGO.GetComponent<UITargetAnimation> ();

			UILink uiLink = newLinkGO.GetComponent<UILink> ();
			uiLink.URL = "";
			UIButtonPress buttonPress = newLinkGO.GetComponent<UIButtonPress> ();
			buttonPress.controllerHub = controllerHub;


				newLinkGO.GetComponent<RawImage> ().color = new Color (1, 1, 1, 0.0f);
				newLinkGO.GetComponent<RawImage> ().raycastTarget = false;
				buttonPress.enabled = false;


			//}

		}

	}

	// Use this for initialization
	void Start () {

		execute (this, "buildLinkArray");
		execute (canvasHub.galleryCanvas, "SetActive", true);
		execute (canvasHub.titleCanvas, "SetActive", false);
		execute (controllerHub.uiController, "fadeIn");


		createSubprogram ("Exitting");

		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (this, "cleanUp");
		execute (canvasHub.galleryCanvas, "SetActive", false);
		execute (canvasHub.titleCanvas, "SetActive", true);
		programNotifyFinish ();



	}

	// UI Callback
	public void backButtonTouch() {
		goTo ("Exitting");
	}
	

}
