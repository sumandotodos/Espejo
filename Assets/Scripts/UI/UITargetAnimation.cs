using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetAnimation : MonoBehaviour {

	public float animationSpeed = 2.0f;

	public Texture[] imgs;
	public RawImage theRawImage;

	float elapsedTime;

	int frame;
	int targetFrame;

	public void setFrame(int tf) {
		frame = targetFrame = tf;
		theRawImage.texture = imgs [frame%imgs.Length];
	}

	public void setTargetFrame(int tf) {
		targetFrame = tf;
	}

	// Use this for initialization
	public void Start () {
		elapsedTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
		elapsedTime += Time.deltaTime;
		if (elapsedTime > (1.0f / animationSpeed)) {
			if (frame != targetFrame) {
				frame = (frame + 1) % imgs.Length;
				theRawImage.texture = imgs [frame];
			}
			elapsedTime = 0.0f;
		}

	}
}
