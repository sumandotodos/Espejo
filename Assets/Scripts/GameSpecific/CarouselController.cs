using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselController : MonoBehaviour {

	public TouchController controller;

	public float effectiveAngle;
	float angle = 0;
	float offset = 0;
	float angleFactor = -60.0f;
	float speedFactor = -1.0f;
	public float angleSpeed;
	public float angleFriction = 2.0f;
	public float targetAngle;
	public bool targetAngleReady;
	public float speedThreshold = 2.0f;
	public float killThreshold = 1.0f;
	public float targetAngleFactor = 0.05f;

	bool isTouching = false;

	void Start() {
		
		targetAngle = -1.0f;
		targetAngleReady = false;

	}

	public float closestAngle(float angle, float speed) {

		float minDistance = 360.0f;
		int minAngle = 0;
		for (int i = 0; i < 6; ++i) {

			float testAngle = ((float)i) * (360.0f / 6.0f);
			if (Mathf.Abs (testAngle - angle) < minDistance) {
				minDistance = Mathf.Abs (testAngle - angle);
				minAngle = i;
			}

		}

		float closestAngle = ((float)minAngle) * (360.0f / 6.0f);
		/*
		if ((closestAngle - angle) > 0.0f) {
			if (speed < 0.0f) {
				minAngle--;
			}
		} else {
			if (speed > 0.0f)
				minAngle++;
		}*/

		return ((float)minAngle) * (360.0f / 6.0f);

	}

	public int whichPlayer() {

		if (targetAngleReady) {
			return (Mathf.FloorToInt (targetAngle / 60.0f) % GameController.MaxPlayers);
		} else
			return -1;

	}

	// Update is called once per frame
	void Update () {

		this.transform.rotation = Quaternion.Euler (0, angle+offset, 0);

		effectiveAngle = angle + offset;

		if (!isTouching) {

			angle += angleSpeed;

			if (Mathf.Abs (angleSpeed) < speedThreshold) {
				targetAngle = closestAngle (angle, angleSpeed);
				targetAngleReady = true;
			} else {
				targetAngleReady = false;
			}

			if (targetAngleReady) {
				angleSpeed = (targetAngle - angle) * targetAngleFactor;
				if (Mathf.Abs(angleSpeed) < killThreshold) {
					angleSpeed = 0.0f;
					angle = targetAngle;
					offset = 0.0f;
				}
			}

			if (controller.isTouching) {
				isTouching = true;
			}
		}

		if (isTouching) {

			angleSpeed = 0.0f;

			offset = angleFactor * controller.deltaX;
			if (!controller.isTouching) {
				angle += offset;
				offset = 0;
				isTouching = false;
				angleSpeed = speedFactor * controller.exitSpeed;
			}
		}

		float absoluteSpeed = Mathf.Abs (angleSpeed);
		absoluteSpeed -= (angleFriction * Time.deltaTime);
		if (absoluteSpeed < 0.0f) {
			absoluteSpeed = 0.0f;
		}
		if (angleSpeed > 0)
			angleSpeed = absoluteSpeed;
		else
			angleSpeed = -absoluteSpeed;
		

		if (angle > 360.0f) {
			angle -= 360.0f;
			if (targetAngle > 360.0) {
				targetAngle -= 360.0f;
			}
		}
		
		if (angle < 0.0f) {
			angle += 360.0f;
			if (targetAngle < 0.0f) {
				targetAngle += 360.0f;
			}
		}



	}
}
