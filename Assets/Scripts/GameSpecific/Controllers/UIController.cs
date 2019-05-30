using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : FGProgram {

	public UIFader globalFader;
	public RawImage wait;

	// Use this for initialization
	void Start () {
		wait.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void fadeIn() {
		globalFader.Start ();
		globalFader.fadeToTransparent ();
	}

	public void fadeOut() {
		globalFader.Start ();
		globalFader.fadeToOpaque ();
	}

	public void fadeOutTask(FGProgram prog) {
		globalFader.Start ();
		globalFader.fadeToOpaqueTask(prog);
	}

	public void startWait() {
		wait.enabled = true;
	}

	public void endWait() {
		wait.enabled = false;
	}
}
