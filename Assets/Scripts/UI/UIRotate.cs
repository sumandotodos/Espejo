﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotate : MonoBehaviour {

	public float speed;
	
	// Update is called once per frame
	void Update () {

		this.transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));

	}
}
