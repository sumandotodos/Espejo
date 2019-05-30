using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum HighlightMode { toggle, pressOnce }
public enum HighlightPolicy { changeTexture, fade }

public class UIHighlight : MonoBehaviour {

	public HighlightMode mode;
	public HighlightPolicy policy;
	public Texture normalTex;
	public Texture highlitTex;

	public float fadedOpacity = 0.35f;

	public UIHighlight[] disableOthers;

	public UIHighlight[] unpressOthers;

	RawImage img;

	bool isEnabled;

	[HideInInspector]
	public bool isHighlit;

	bool started = false;

	public void Start() {

		if (started)
			return;
		started = true;

		img = this.GetComponent<RawImage>();
		isEnabled = true;
		isHighlit = false;
	}

	public void touchCallback() {

		if (!isEnabled)
			return;

		if (mode == HighlightMode.pressOnce) {

			if (policy == HighlightPolicy.changeTexture) {
				img.texture = highlitTex;
				isHighlit = true;
			} else {
				img.color = new Color (1, 1, 1, fadedOpacity);
				isHighlit = true;
			}

		} 
		else {

			if (policy == HighlightPolicy.changeTexture) {
				if (img.texture == highlitTex) {
					img.texture = normalTex;
					isHighlit = false;
				} else {
					img.texture = highlitTex;
					isHighlit = true;
				}
			} else {
				if (isHighlit == true) {
					img.color = new Color (1, 1, 1, 1);
					isHighlit = false;
				} else {
					img.color = new Color (1, 1, 1, fadedOpacity);
					isHighlit = true;
				}
			}
		}

		for (int i = 0; i < disableOthers.Length; ++i) {
			disableOthers [i].disable ();
		}

		for (int i = 0; i < unpressOthers.Length; ++i) {
			unpressOthers[i].unpress ();
		}

	}

	public void unpress() {
		if (!isEnabled)
			return;

		if (policy == HighlightPolicy.changeTexture) {
			img.texture = normalTex;
			isHighlit = false;
		} else {
			img.color = new Color (1, 1, 1, 1);
			isHighlit = false;
		}
	}

	public void disable() {
		isEnabled = false;
	}

	public void setEnable(bool en) {
		isEnabled = en;
	}
}
