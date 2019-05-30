using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DismissType {  dismissForGame, doNotDismiss };

public class UITip : MonoBehaviour {

	public bool autostart = true;
	public UITip nextTip;


	public DismissType dismissType = DismissType.doNotDismiss;

	public string tipID;

	GameController gameController;

	UIGeneralFader generalFader;

	bool visible;
	bool waitingToVanish;

	public IEnumerator launchCoRo() {
		yield return new WaitForSeconds (0.5f);
		generalFader.fadeToOpaque ();
		this.GetComponentInChildren<Image> ().raycastTarget = true;
		yield return new WaitForSeconds (0.5f);
		visible = true;
	}

	int state = -1;
	// Use this for initialization


	bool shown = false;
	void OnEnable() {
		Start ();
	}

	void Start () {

		generalFader = this.GetComponent<UIGeneralFader> ();

		this.GetComponentInChildren<Text> ().raycastTarget = false;
		this.GetComponentInChildren<Image> ().raycastTarget = false;

		generalFader.Start ();
		generalFader.fadeOutImmediately ();

		state = -1;
		visible = false;
		waitingToVanish = false;

		if (autostart) {

			show ();	

		}
	}


	bool mustLauchCoRo = false;
	public void show() {
		if (shown)
			return;
		if (!GameObject.Find ("GameController").GetComponent<GameController> ().tipSaveData.dismissedTips.Contains (tipID)) {
			mustLauchCoRo = true;
			shown = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (state == -1) {
			generalFader.Start ();
			generalFader.fadeOutImmediately ();
			state = 0;
		}
		if (mustLauchCoRo) {
			StartCoroutine ("launchCoRo");
			mustLauchCoRo = false;
		}

	}

	public void dismissTip() {
		if (visible) {
			visible = false;
			shown = false;
			if (nextTip) {
				nextTip.show ();
			}
			generalFader.fadeToTransparent ();
			this.GetComponentInChildren<Image> ().raycastTarget = false;
			if (dismissType == DismissType.dismissForGame) {
				if (!tipID.Equals ("")) {
					GameObject.Find ("GameController").GetComponent<GameController> ().tipSaveData.dismissedTips.Add (tipID);
				}
			}
		}
	}
}
