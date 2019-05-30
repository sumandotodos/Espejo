using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToggleMode { toggle, justOnce };

public class UIImageToggle : MonoBehaviour {

	public RawImage rawImage;

	public bool mustTurnOffOthers = true;
	public bool mustDisableOthers = true;

	public UIImageToggle[] offOthers_N;
	public UIImageToggle[] disableOthers_N;

	public Texture offImg;
	public Texture onImg;

	public bool defaultOn;
	public ToggleMode toggleMode;

	public bool started = false;
	public bool enabled = true;

	public void reset() {
		
		if (defaultOn) {
			rawImage.texture = onImg;

		} else
			rawImage.texture = offImg;

		enabled = true;

	}

	public void off() {
		rawImage.texture = offImg;
	}

	public void setEnabled(bool en) {
		enabled = en;
	}

	public void toggle() {

		if (!enabled)
			return;

		if (toggleMode == ToggleMode.toggle) {
			if (rawImage.texture == onImg) {
				rawImage.texture = offImg;
			} else
				rawImage.texture = onImg;
		} else {
			rawImage.texture = onImg;
		}

		if (rawImage.texture == onImg) {
			if (mustTurnOffOthers) {
				for (int i = 0; i < offOthers_N.Length; ++i) {
					offOthers_N [i].off ();
				}
			}
			if (mustDisableOthers) {
				for (int i = 0; i < disableOthers_N.Length; ++i) {
					disableOthers_N [i].setEnabled (false);
				}
			}
		}



	}

	public bool status() {
		return rawImage.texture == onImg;
	}

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
