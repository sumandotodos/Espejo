using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastTypeController : MonoBehaviour {

	public Text output_N;

	public int lastType = -1;
	public int sameTypeTimes = 0;

	public void updateLastType(int t) {
		if (lastType != t)
			sameTypeTimes = 0;
		lastType = t;
	}

	void Update() {
		if (output_N != null)
			output_N.text = lastType + "\n" + sameTypeTimes;
	}

}
