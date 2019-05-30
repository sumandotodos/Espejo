using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LeftOrRight { left, right };

public class ArrowAuxButton : MonoBehaviour, ButtonPressListener {

	public LeftOrRight direction;

	public ScreenIController screenIController;

	public void buttonPress() {

		if (direction == LeftOrRight.left) {
			screenIController.scrollLeft ();
		} else {
			screenIController.scrollRight ();
		}

	}

}
