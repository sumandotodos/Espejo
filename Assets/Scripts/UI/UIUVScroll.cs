using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUVScroll : MonoBehaviour {

	public float uSpeed = 0;
	public float vSpeed = 0;

	public RawImage theImage;

	// Update is called once per frame
	void Update () {

		Rect uvCoords = theImage.uvRect;
		uvCoords.x += uSpeed * Time.deltaTime;
		uvCoords.y += vSpeed * Time.deltaTime;
		theImage.uvRect = uvCoords;

	}
}
