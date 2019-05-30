using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILink : MonoBehaviour, ButtonPressListener {

	public string URL;
	public float remainingTime = 0.0f;
	public float delay = 0.25f;

	public void buttonPress() {
		Debug.Log ("Link key!:" + URL);
		remainingTime = delay;

	}

	void Update() {

		if (remainingTime > 0.0f) {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0.0f) {
				
				Application.OpenURL (URL);
			}
		}

	}

}
