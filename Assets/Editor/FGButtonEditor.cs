//#define PLAYINEDITOR

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FGButton))]
public class FGButtonEditor : Editor  {


	public override void OnInspectorGUI() {

		DrawDefaultInspector ();

		FGButton butRef = (FGButton)target;

		GameObject RosettaGO = GameObject.Find ("Rosetta");
		if (RosettaGO != null)
			butRef.rosettaWrapper.rosetta = RosettaGO.GetComponent<Rosetta> ();

		if (GUILayout.Button ("Upload to rosetta")) {
			if (butRef.rosettaWrapper.rosetta != null) {
				//butRef.rosetta.registerString (butRef.key, butRef.text);
				Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/Rosetta.prefab");
				PrefabUtility.ReplacePrefab (butRef.rosettaWrapper.rosetta.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);


			}
		}

	}

}

#endif