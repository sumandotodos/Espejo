using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITextStimulus : MonoBehaviour {

	public UITextFader fader;

	public float stimulusDuration;
	float elapsedTime;
	int state = 0;

	// Use this for initialization
	void Start () {
		elapsedTime = 0;	
	}

	public void reset() {
		state = 0;
		elapsedTime = stimulusDuration * 2;
		fader.Start ();
		fader.setOpacity (0.0f);
	}

	public void stimulate() {
		elapsedTime = 0;
		state = 1;
		fader.Start ();
		fader.setOpacity (1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (state == 0) {
			return;
		}
		if (state == 1) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime > stimulusDuration) {
				fader.fadeToTransparent ();
				state = 0;
			}
		}
	}
}
