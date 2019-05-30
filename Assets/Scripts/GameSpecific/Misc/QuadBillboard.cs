using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadBillboard : MonoBehaviour {

	public Camera m_Camera;

	void OnDrawGizmos() {

		if (m_Camera != null) {
			Update ();
		}

	}

	public void Update()
	{
		transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
			m_Camera.transform.rotation * Vector3.up);
	}
}
