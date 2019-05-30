using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIFader : FGProgram {

	public float prevValue;
	public float value;

	public float opacityValue;
	public float opacityTarget;

	public SoftFloat opacity;
	bool started = false;
	RawImage rimageComponent;
	Image imageComponent;
	public float maxOpacity = 1.0f;
	public float minOpacity = 0.0f;
	public float speed = 1.0f;
	public Color opaqueColor = Color.black;
	public bool startOpaque = true;

	int state = 0; 	// 0: idle;
					// 1: fading

	private void updateColor() {
		Color newColor = opaqueColor;
		newColor.a = opacity.getValue ();
		if(imageComponent)
		imageComponent.color = newColor;
		if (rimageComponent)
			rimageComponent.color = newColor;
	}

	public void Start() {
		if (started == true)
			return;
		started = true;
		opacity = new SoftFloat ();
		//opacity.setTransformation (TweenTransforms.tanh);
		rimageComponent = this.GetComponent<RawImage> ();
		imageComponent = this.GetComponent<Image> ();
		if (startOpaque) {
			opacity.setValueImmediate (maxOpacity);
		} else {
			opacity.setValueImmediate (minOpacity);
		}
		opacity.setSpeed (speed);
		state = 0;
		updateColor ();
	}

	public void setOpacity(float op) {
		if (op > 0.0f) {
			if (imageComponent != null)
				imageComponent.enabled = true;
			if (rimageComponent != null)
				rimageComponent.enabled = true;
		} else {
			if(imageComponent != null) 
			imageComponent.enabled = false;
			if (rimageComponent != null)
				rimageComponent.enabled = false;
		}
		opacity.setValueImmediate (op);
		opacityValue = op;
		updateColor ();
	}

	public void fadeToOpaque() {
		state = 1;
		if (imageComponent != null)
			imageComponent.enabled = true;
		if (rimageComponent != null)
			rimageComponent.enabled = true;
		opacity.setValue (maxOpacity);
	}

	public void fadeToTransparent() {
		state = 1;
		if(imageComponent != null) 
		imageComponent.enabled = true;
		if (rimageComponent != null)
			rimageComponent.enabled = true;
		opacity.setValue (minOpacity);
	}

	public void fadeToOpaqueTask(FGProgram waiter) {
		registerWaiter (waiter);
		fadeToOpaque ();
	}

	public void fadeToTransparentTask(FGProgram waiter) {
		registerWaiter (waiter);
		fadeToTransparent ();
	}

	void Update() {

		opacityValue = opacity.getValue ();//value;
		opacityTarget = opacity.linSpaceTarget;

		//update (); // update program
		prevValue = opacity.prevValue;
		value = opacity.getValue ();//.value;
		if(state == 1) {
			if (!opacity.update ()) {
				if (opacity.getValue () == 0.0f) {
					if(imageComponent != null)
					imageComponent.enabled = false;
					if (rimageComponent != null)
						rimageComponent.enabled = false;
				}
				notifyFinishExternal ();
				state = 0;
			} 
			updateColor ();

		}

	}

	public void notifyFinishExternal() {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].waitFinish ();
		}
		waiters = new List<FGProgram>();
	}
		
}
