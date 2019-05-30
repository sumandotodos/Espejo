using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFadeInOnDelay : MonoBehaviour {

	UITextFader textFader;
	UIFader imageFader;

	public float delay;
	float timer;

	// Use this for initialization
	void Start () {
		textFader = this.GetComponent<UITextFader> ();
		imageFader = this.GetComponent<UIFader> ();
		timer = delay;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0f) {
			timer -= Time.deltaTime;
			if (timer <= 0f) {
				if(textFader != null) textFader.fadeToOpaque ();
				if(imageFader != null) imageFader.fadeToOpaque ();
			}
		}
	}
}
