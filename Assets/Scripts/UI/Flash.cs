using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour {

	public RawImage theImage;
	public float activeTime = 0.15f;
	float remainingTime;

	// Use this for initialization
	void Start () {
		theImage.enabled = false;
		remainingTime = 0.0f;
	}

	public void flash() {
		remainingTime = activeTime;
		theImage.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (remainingTime > 0.0f) {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0.0f) {
				theImage.enabled = false;
			}
		}

	}
}
