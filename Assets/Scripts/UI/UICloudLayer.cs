using UnityEngine;
using System.Collections;

public class UICloudLayer : MonoBehaviour {

	public float width;
	public float scrollSpeed;
	float x;
	Vector3 initialPos;

	// Use this for initialization
	void Start () {
	
		initialPos = this.transform.localPosition;
		x = initialPos.x;

	}
	
	// Update is called once per frame
	void Update () {
	
		x += scrollSpeed * Time.deltaTime;
		if (x > (width))
			x -= (2 * width);
		if( x < (-width)) {
			x += (2 * width);
		}
		Vector3 pos = initialPos;
		pos.x = x;
		this.transform.localPosition = pos;

	}
}
