
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class FGButton : Button {

	public string key;

	public RosettaWrapper rosettaWrapper;

	string locale;

	// Use this for initialization
	new void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Add a menu item to create custom GameObjects.
	// Priority 1 ensures it is grouped with the other menu items of the same kind
	// and propagated to the hierarchy dropdown and hierarch context menus. 

	#if UNITY_EDITOR

	[MenuItem("GameObject/UI/FGButton", false, 10)]
	static void CreateCustomGameObject(MenuCommand menuCommand) {
		// Create a custom game object
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/FGButton")); //(GameObject)Instantiate(Resources.Load("FGText"));//new GameObject("FGText"); 
		// Ensure it gets reparented if this was a context click (otherwise does nothing)

		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		go.transform.localScale = Vector3.one;
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	#endif
}
