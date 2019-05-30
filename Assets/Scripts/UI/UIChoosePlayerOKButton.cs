using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChoosePlayerOKButton : MonoBehaviour, ButtonPressListener {

	public CarouselController carouselController;
	public PlayerSelectController playerSelectController;

	public void buttonPress() 
	{
		int player = 0;
		player = carouselController.whichPlayer ();
		if (player != -1) {
			playerSelectController.buttonPress (player);
		}
	}

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
