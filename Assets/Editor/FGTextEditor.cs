

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FGText))]
public class FGTextEditor : Editor  {
	

	public override void OnInspectorGUI() {

		DrawDefaultInspector ();

		FGText textRef = (FGText)target;

		GameObject RosettaGO = GameObject.Find ("Rosetta");
		if (RosettaGO != null)
			textRef.rosettaWrapper.rosetta = RosettaGO.GetComponent<Rosetta> ();

		if (GUILayout.Button ("Upload to rosetta")) {
			if (textRef.rosettaWrapper.rosetta != null) {
				textRef.rosettaWrapper.rosetta.registerString (textRef.key, textRef.text);
				Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/Rosetta.prefab");
				PrefabUtility.ReplacePrefab (textRef.rosettaWrapper.rosetta.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);


			}
		}

	}

}

#endif