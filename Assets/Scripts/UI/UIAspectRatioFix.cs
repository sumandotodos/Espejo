using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AspectFixMode { scaleToFit, fitWidth };

public class UIAspectRatioFix : MonoBehaviour {

	public float aspect;
	public GameObject extragnoAdjust;

	Vector3 localScale;

	public AspectFixMode fixMode = AspectFixMode.scaleToFit;

	// Use this for initialization
	void Awake () {
		localScale = this.transform.localScale;
		float scale = 1;
		aspect = ((float)Screen.height) / ((float)Screen.width);
		if (aspect > 1.78f) {
			scale = -0.4485488f * aspect + 1.797071f;
		}
		if (fixMode == AspectFixMode.scaleToFit) {
			this.transform.localScale = new Vector3 (scale*localScale.x, scale*localScale.y, scale*localScale.z);
		} else {
			float relscate = (16f / 9f) / aspect;
			transform.transform.localScale = new Vector3 (relscate*localScale.x, 1*localScale.y, 1*localScale.z);
		}
		if (extragnoAdjust != null) {
			Vector3 pos = extragnoAdjust.transform.position;
			//			if (aspect > 2f) {
			//				pos -= new Vector3 (84, 0, 0);
			//			} else if (aspect < 1.77f) {
			//				pos -= new Vector3 (12, 0, 0);
			//			} else {
			//				pos -= new Vector3 (59, 0, 0);
			//			}
			float adj = 65.963f * aspect - 58.216f;
			if ((aspect < 1.78f) && (aspect > 1.76f)) { // 16:9
				adj = 59f;
			}
			if ((aspect < 2.18f) && (aspect > 2.14f)) { // iPhone X
				adj = 70;
			}
			if ((aspect < 1.34f) && (aspect > 1.33f)) { // Tres cuartos de lo mismo
				adj = 16f;
			}
			pos -= new Vector3 (adj, 0, 0);
			extragnoAdjust.transform.position = pos;
		}
	}

	public void Start() {
		
	}

}
