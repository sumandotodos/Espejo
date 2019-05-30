
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Autorosetta))]
public class AutorosettaEditor : Editor  {


	public override void OnInspectorGUI() {

		DrawDefaultInspector ();



		Autorosetta AutorosRef = (Autorosetta)target;


		if (GUILayout.Button("Go (Create instances first!)")) {


			AutorosRef.go();


		}

	}


}


#endif