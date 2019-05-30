using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public AudioSource aSource;

	public void playSound(AudioClip sound) {
		if(sound!=null)
		aSource.PlayOneShot (sound);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
