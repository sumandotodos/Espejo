using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDelayFader : MonoBehaviour {

	public UIFader fader;
	public float delay;
	float remainingTime;

	public bool going = true;

	public bool fadeIn = true;

	public void resetTimer() {
		remainingTime = delay;
	}

	// Use this for initialization
	void Start () {
		remainingTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (going) {
			if (remainingTime > 0.0f) {
				remainingTime -= Time.deltaTime;
				if (remainingTime <= 0.0f) {
					if (fadeIn)
						fader.fadeToOpaque ();
					else
						fader.fadeToTransparent ();
				}
			}
		}
	}
}
