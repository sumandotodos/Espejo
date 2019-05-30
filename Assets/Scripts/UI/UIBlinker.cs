using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlinker : MonoBehaviour {

	public RawImage image;
	public float frequency;
	public bool startVisible = false;

	int state;
	float timer;

	// Use this for initialization
	public void Start () {
		state = 0;
		timer = 0.0f;
		image.enabled = startVisible;
	}

	public void startBlinking() {
		state = 1;
	}

	public void stopBlinking(bool visible) {
		state = 0;
		image.enabled = visible;
	}

	// Update is called once per frame
	void Update () {
		if (state == 0) {

		}

		if (state == 1) {
			timer += Time.deltaTime;
			if (timer > (1.0f / frequency) / 2.0f) {
				state = 2;
				image.enabled = true;
				timer = 0.0f;
			}
		}

		if (state == 2) {
			timer += Time.deltaTime;
			if (timer > (1.0f / frequency) / 2.0f) {
				state = 1;
				image.enabled = false;
				timer = 0.0f;
			}
		}
	}
}
