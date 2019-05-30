using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnableImageOnTimeout : MonoBehaviour {

	public float timeout;
	float remainingTime;

	RawImage theImage;
	public Text text;

	public bool going = false;
	public RawImage cubreTodo;
	float time;

	void Start () 
	{
		remainingTime = timeout;
		theImage = this.GetComponent<RawImage> ();
		theImage.enabled = false;
		going = false;
		//text = this.GetComponentInChildren<Text> ();
		//text.enabled = false;
		text.gameObject.SetActive (false);
		cubreTodo.enabled = false;
	}
	
	void Update ()
	{
		if (!going)
			return;

		if (remainingTime > 0.0f) {

			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0.0f)
			{
				theImage.enabled = true;
				text.enabled = true;
				cubreTodo.enabled = true;
				text.gameObject.SetActive (true);
				text.GetComponent<UITextBlinker> ().startBlink ();
			}
		}
	}

	public void go() {
		going = true;
	}

	public void stop() {
		going = false;
		keepAlive ();
	}

	public void keepAlive() {
		theImage.enabled = false;
		cubreTodo.enabled = false;
		text.GetComponent<UITextBlinker> ().disable ();
		text.GetComponent<UITextBlinker> ().stopBlink ();
		text.gameObject.SetActive(false);
		//text.enabled = false;
		remainingTime = timeout;
	}
}
