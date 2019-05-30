using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISoftBlinker : MonoBehaviour {

	SoftFloat opacity;
	public float speed;
	public bool startHidden = true;
	public RawImage theImage;

	public bool stopped = true;
	public int turn;
	public float value;

	public bool started = false;

	void updateOpacity() {
		theImage.color = new Color (1, 1, 1, opacity.getValue ());
	}

	public void reset() {
		if (startHidden) {
			opacity.setValueImmediate (0.0f);
			turn = 0;
		} else {
			opacity.setValueImmediate (1.0f);
			turn = 2;
		}
		stopped = true;
		opacity.setSpeed (speed);
		updateOpacity ();
	}

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		opacity = new SoftFloat ();
		reset ();
	}

	public void go() {
		stopped = false;
	}

	
	// Update is called once per frame
	void Update () {

		if (stopped)
			return;

		updateOpacity ();
		value = opacity.getValue ();

		if (turn == 0) {
			turn = 1;
			opacity.setValue (1.0f);
		}
		if (turn == 1) {
			if (!opacity.update ()) {
				turn = 2;
			}
		}
		if (turn == 2) {
			turn = 3;
			opacity.setValue (0.0f);
		}
		if (turn == 3) {
			if (!opacity.update ()) {
				turn = 0;
			}
		}

	}
}
