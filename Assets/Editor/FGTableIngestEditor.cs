using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(FGTableIngest))]
public class FGTableIngestEditor : Editor  {


	public override void OnInspectorGUI() 
	{
		DrawDefaultInspector ();

		FGTableIngest ingestRef = (FGTableIngest)target;

		if (GUILayout.Button ("Load file")) 
		{
			string path = EditorUtility.OpenFilePanel ("Choose file", "", "txt");
			StreamReader fileIn = new StreamReader (path);
			string contents = fileIn.ReadToEnd ();
			ingestRef.loadFile (contents);
		}
		if (ingestRef.fileLoaded) {
			if (GUILayout.Button ("Process"))
			{
				parse (ingestRef);
			}
		}
	}

	public void parse(FGTableIngest t) {

		int nColumns = 1;

		string[] files = t.fileContents.Split ('\n');
		int n;
		if (int.TryParse (files [0], out n)) {
			nColumns = n;
		}




		//prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/StringBanks/" + ingestRef.outputFolder + "/StringBank(Preguntas" + setName + ").prefab");
		//GO = PrefabUtility.ReplacePrefab (newStringBankGO, prefab, ReplacePrefabOptions.ConnectToPrefab);

	}
}