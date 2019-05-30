using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

	public string scene;

	// Use this for initialization
	IEnumerator Start () {
		AsyncOperation loadAll = SceneManager.LoadSceneAsync ("Scenes/" + scene);
		yield return loadAll;
	}
	

}
