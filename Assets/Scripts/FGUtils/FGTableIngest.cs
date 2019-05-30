using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGTableIngest : MonoBehaviour {

	[HideInInspector]
	public string fileContents = "";
	[HideInInspector]
	public bool fileLoaded = false;

	public string outputFolder;

	public void loadFile(string contents) {
		fileContents = contents;
		fileLoaded = true;
	}

	public void process() {

	}


}
