using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHover : MonoBehaviour {

	public float speed1 = 18;
	public float speed2 = 25;

	public float amplitude = 10.0f;

	public float angle1;
	public float angle2;

	Vector3 startPos;
	Vector3 delta;

	// Use this for initialization
	void Start () {

		if(angle1==0f)
		angle1 = Random.Range (0.0f, 360.0f);
		if(angle2==0f)
		angle2 = Random.Range (0.0f, 360.0f);

		startPos = this.transform.localPosition;

	}
	
	// Update is called once per frame
	void Update () {

		angle1 += Time.deltaTime * speed1;
		angle2 += Time.deltaTime * speed2;

		delta = new Vector3 (Mathf.Cos (angle1), Mathf.Sin (angle2), 0) * amplitude;
		this.transform.localPosition = startPos + delta;

	}
}
