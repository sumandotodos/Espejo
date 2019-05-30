using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGeneralFader : MonoBehaviour {

	UIFader[] imageFaders;
	UITextFader[] textFaders;

	// Use this for initialization
	bool started = false;
	public void Start () {
		if (started)
			return;
		started = true;
		imageFaders = this.GetComponentsInChildren<UIFader> ();
		textFaders = this.GetComponentsInChildren<UITextFader> ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void fadeToOpaque() {
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.fadeToOpaque ();
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.fadeToOpaque ();
		}
	}

	public void fadeToTransparent() {
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.fadeToTransparent ();
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.fadeToTransparent ();
		}
	}

	public void fadeOutImmediately() {
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.setOpacity (0f);
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.setOpacity (0f);
		}
	}
}
