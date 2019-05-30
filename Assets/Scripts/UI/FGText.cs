using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FGText : Text {

	public string key;

	public RosettaWrapper rosettaWrapper;

	string locale = "default";

	void Start() {

		if (rosettaWrapper == null)
			rosettaWrapper = GameObject.Find ("RosettaWrapper").GetComponent<RosettaWrapper> ();

		this.text = rosettaWrapper.rosetta.retrieveString (key);

		locale = rosettaWrapper.rosetta.locale ();

	}

	void Update() {
		if (rosettaWrapper.rosetta != null) {
			string loc;
			if (rosettaWrapper.rosetta.locale () != null) {
				loc = rosettaWrapper.rosetta.locale ();
			} else
				loc = "default";
			if (!loc.Equals (locale)) {
				this.text = rosettaWrapper.rosetta.retrieveString (key);
				locale = rosettaWrapper.rosetta.locale ();
			}
		}
	}


	#if UNITY_EDITOR
	// Add a menu item to create custom GameObjects.
	// Priority 1 ensures it is grouped with the other menu items of the same kind
	// and propagated to the hierarchy dropdown and hierarch context menus. 
	[MenuItem("GameObject/UI/FGText", false, 10)]
	static void CreateCustomGameObject(MenuCommand menuCommand) {
		// Create a custom game object
		GameObject go = new GameObject("FGText"); //(GameObject)Instantiate(Resources.Load("FGText"));//new GameObject("FGText"); 
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		go.AddComponent<FGText>();
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
	#endif
}
