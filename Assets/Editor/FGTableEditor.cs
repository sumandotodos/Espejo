#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[CustomEditor(typeof(FGTable))]
public class FGTableEditor : Editor {




	public override void OnInspectorGUI() {


		FGTable tableRef = (FGTable)target;

		if (tableRef.rosettaWrapper == null) {
			GameObject rosettaGO = GameObject.Find ("RosettaWrapper");
			if (rosettaGO != null) {
				tableRef.rosettaWrapper = rosettaGO.GetComponent<RosettaWrapper> ();
			}
		}


		if ((tableRef.column.Count > 0) && (tableRef.column [0].nItems() > 0)) {
			tableRef.cols = tableRef.nColumns ();
			tableRef.rows = tableRef.column [0].nItems ();
		} 

		DrawDefaultInspector ();

		if (GUILayout.Button ("Import from CRSV")) {

			string path = EditorUtility.OpenFilePanel ("Choose file", "", "crsv,txt");
			if (!path.Equals("")) {
				StreamReader fileIn = new StreamReader (path);
				string contents = fileIn.ReadToEnd ();
				//ingestRef.loadFile (contents);
				tableRef.importCRSV (contents);
			}

		}

		if (GUILayout.Button ("Import from JSON")) {

			string path = EditorUtility.OpenFilePanel ("Choose file", "", "json,txt");
			if (!path.Equals("")) {
				StreamReader fileIn = new StreamReader (path);
				string contents = fileIn.ReadToEnd ();
				//ingestRef.loadFile (contents);
				string crsvRep = tableRef.JSON2CRSV (contents);
				//EditorGUIUtility.systemCopyBuffer = crsvRep;
				tableRef.importCRSV (crsvRep);
			}

		}

		if (GUILayout.Button ("Import from JSON (Emparchis only)")) {

			string path = EditorUtility.OpenFilePanel ("Choose file", "", "json,txt");
			if (!path.Equals("")) {
				StreamReader fileIn = new StreamReader (path);
				string contents = fileIn.ReadToEnd ();
				//ingestRef.loadFile (contents);
				string crsvRep = tableRef.importJSONEmparchis (contents);
				EditorGUIUtility.systemCopyBuffer = crsvRep;
				tableRef.importCRSV (crsvRep);
			}

		}

		if (GUILayout.Button ("Export to CRSV")) {

			EditorGUIUtility.systemCopyBuffer = tableRef.exportCRSV ();

		}

		if (GUILayout.Button ("Export to JSON")) {

			EditorGUIUtility.systemCopyBuffer = tableRef.exportJSON ();

		}

//		if (GUILayout.Button ("Import from JSON")) {
//
//			string path = EditorUtility.OpenFilePanel ("Choose file", "", "json,txt");
//			if (!path.Equals("")) {
//				StreamReader fileIn = new StreamReader (path);
//				string contents = fileIn.ReadToEnd ();
//				//ingestRef.loadFile (contents);
//				tableRef.importJSON (contents);
//			}
//
//		}

		if (GUILayout.Button ("Reset")) {


			tableRef.reset ();

		}

//		if (GUILayout.Button ("Push to Rosetta")) {
//
//			for (int i = 0; i < tableRef.column.Length; ++i) {
//				StringBank column = tableRef.column [i];
//				if (column != null) {
//					column.extra = tableRef.globalExtra + "_" + i;
//					column.rosetta = tableRef.rosetta;
//					for (int j = 0; j < column.phrase.Length; ++j) {
//
//						column.rosetta.registerString (column.extra + "_" + j, column.phrase [j]);
//
//					}
//				}
//			}
//
//		}

	}


}

#endif