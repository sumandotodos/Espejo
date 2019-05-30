using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureToggle : MonoBehaviour {

	public Texture onTexture;
	public Texture offTexture;

	Material mat;

	public void toggleTexture() {
		mat.mainTexture = offTexture;
	}

	// Use this for initialization
	void Start () {
		mat = this.GetComponent<Renderer> ().material; // instance, please
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
