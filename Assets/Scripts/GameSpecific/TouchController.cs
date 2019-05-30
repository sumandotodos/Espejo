using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

	public bool isTouching;
	public float deltaX;
	public Vector3 touchPoint;
	public Vector3 previousTouchPoint;
	public Vector3 currentTouchPoint;
	Vector3 releasePoint;
	public float exitSpeed;
	public float deltaTime = 0.05f;
	public float maxExitSpeed = 30.0f;
	float elapsedTime;

	// Use this for initialization
	void Start () {

		elapsedTime = 0.0f;
		isTouching = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (!isTouching) {

			if (Input.GetMouseButtonDown (0)) {
				previousTouchPoint = currentTouchPoint = touchPoint = Input.mousePosition / Screen.width;
				isTouching = true;
			}

			deltaX = 0;

		}

		if (isTouching) {

			elapsedTime += Time.deltaTime;
			if (elapsedTime > deltaTime) {
				previousTouchPoint = currentTouchPoint; // update previous point
				elapsedTime = 0.0f;
			}
			currentTouchPoint = Input.mousePosition / Screen.width;

			deltaX = currentTouchPoint.x - touchPoint.x;

			if (Input.GetMouseButtonUp (0)) {
				releasePoint = currentTouchPoint;
				isTouching = false;
				exitSpeed = (currentTouchPoint.x - previousTouchPoint.x) / Time.deltaTime;
				if (Mathf.Abs (exitSpeed) > maxExitSpeed) {
					if (exitSpeed > 0.0f)
						exitSpeed = maxExitSpeed;
					else
						exitSpeed = -maxExitSpeed;
				}
			}

		}

	}
}
