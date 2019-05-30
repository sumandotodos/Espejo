using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOpacityWiggle : MonoBehaviour {

	float opacity;

	public float maxOpacity = 0.75f;

	public float angSpeed = 6.0f;

	float angle;

	bool isActive;

	RawImage img;

	public void reset() {
		opacity = 0.0f;
		isActive = false;
		angle = 0.0f;
		img = this.GetComponent<RawImage> ();
		img.color = new Color (1, 1, 1, 0);
	}

	// Use this for initialization
	void Start () {
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!isActive)
			return;

		angle += angSpeed * Time.deltaTime;
		opacity = (maxOpacity / 2.0f) + (maxOpacity / 2.0f) * Mathf.Sin (angle);
		img.color = new Color (1, 1, 1, opacity);

	}

	public void go() {
		isActive = true;
	}
}
