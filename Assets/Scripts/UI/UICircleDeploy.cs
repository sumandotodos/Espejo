using UnityEngine;
using System.Collections;

public class UICircleDeploy : FGProgram {

	public float nElements = 7;
	public int elementIndex = 0;

	public float maxRadius = 60.0f;
	public float speed = 4.0f;

	public float yScale = 1.0f;

	Vector2 originalPosition;
	Vector2 displacement;

	public bool startExtended = false;

	//public ClickEventReceiver clickEventReceiver_N;

	float radius;
	public float angle;

	int state;

	float initialScale;


	// Use this for initialization
	public void Start () {
	
		initialScale = 1;
		originalPosition = this.transform.localPosition;
		reset ();

	}

	public void setNElements(int n) {
		nElements = n;
		angle = (2.0f * (float)Mathf.PI) * ((float)elementIndex / (float)nElements);
	}

	public void setIndex(int i) {
		elementIndex = i;
		angle = (2.0f * (float)Mathf.PI) * ((float)elementIndex / (float)nElements);
	}
		

	public void reset() {

		if (startExtended) {
			radius = maxRadius;
			state = 2;
		} else {
			radius = 0.0f;
		}

		angle = (2.0f * (float)Mathf.PI) * ((float)elementIndex / (float)nElements);
		this.transform.localScale = new Vector3 (initialScale*(radius / maxRadius), 
			initialScale*(radius / maxRadius), 
			initialScale*(radius / maxRadius));
	}

	// Update is called once per frame
	void Update () {
	
		if (state == 0) { // idle

		}

		if (state == 1) { // extending


			radius += speed * Time.deltaTime;
			if (radius > maxRadius) {
				radius = maxRadius;
				notifyFinish();
				state = 2;
			}
			if (nElements > 1) {
				displacement.x = radius * Mathf.Sin (angle);
				displacement.y = radius * Mathf.Cos (angle) * yScale;
			} else {
				displacement = Vector2.zero;
			}
			this.transform.localPosition = originalPosition + displacement;
			this.transform.localScale = new Vector3 (initialScale*(radius / maxRadius), 
				initialScale*(radius / maxRadius), 
				initialScale*(radius / maxRadius));

		}

		if (state == 2) { // extended

		}

		if (state == 3) { // retracting


			radius -= speed * Time.deltaTime;
			if (radius < 0.0f) {
				radius = 0.0f;
				state = 0;
				notifyFinish ();
			}
			if (nElements > 1) {
				displacement.x = radius * Mathf.Sin (angle);
				displacement.y = radius * Mathf.Cos (angle) * yScale;
			} else {
				displacement = Vector2.zero;
			}
			this.transform.localPosition = originalPosition + displacement;
			this.transform.localScale = new Vector3 (initialScale*(radius / maxRadius), 
				initialScale*(radius / maxRadius), 
				initialScale*(radius / maxRadius));

		}

	}

//	public void extendTask(Task w) {
//		w.isWaitingForTaskToComplete = true;
//		waiter = w;
//		extend ();
//	}

	public void extend() {

		state = 1;

	}

//	public void retractTask(Task w) {
//		w.isWaitingForTaskToComplete = true;
//		waiter = w;
//		retract ();
//	}

	public void retract() {

		state = 3;

	}




}
