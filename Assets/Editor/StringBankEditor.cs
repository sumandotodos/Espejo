

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StringBank))]
public class StringBankEditor : Editor {

	int indexOfStringInList(string[] list, string str) {

		for (int i = 0; i < list.Length; ++i) {
			if (str.Equals (list [i]))
				return i;
		}
		return 0;

	}

	const int MAXSBENTRYLENGTH = 10;
	private string compressPhrase(string phrase) {

		phrase = phrase.ToUpper ();
		phrase = phrase.Replace (" ", "");
		phrase = phrase.Replace ("Ñ", "N");
		phrase = phrase.Replace ("Á", "A");
		phrase = phrase.Replace ("É", "E");
		phrase = phrase.Replace ("Ó", "O");
		phrase = phrase.Replace ("Ú", "U");
		phrase = phrase.Replace ("Í", "I");
		phrase = phrase.Replace ("Ü", "U");
		if (phrase.Length > MAXSBENTRYLENGTH)
			phrase = phrase.Substring (0, MAXSBENTRYLENGTH);
		return phrase;

	}

	private string generateConstants(StringBank target) {

		string res = "";
		res += "// SBName: " + target.extra + "\n\n";
		for (int i = 0; i < target.phrase.Length; ++i) {

			res += "const int " + compressPhrase(target.phrase[i]) + " = " + i + ";\n";

		}

		return res;

	}

	public override void OnInspectorGUI() {

		StringBank bankRef = (StringBank)target;

		string[] yieldType = {

			"Serial",
			"Random"

		};
			
		int index;
		if (bankRef.randomYield)
			index = 1;
		else
			index = 0;
		index = EditorGUILayout.Popup (index, yieldType);
		if (index == 0)
			bankRef.randomYield = false;
		else
			bankRef.randomYield = true;



		DrawDefaultInspector ();

		if (GUILayout.Button ("Generate Constants to clipboard")) {

			EditorGUIUtility.systemCopyBuffer = generateConstants (bankRef);

		}

		if (GUILayout.Button ("Update Rosetta")) {

			for (int i = 0; i < bankRef.phrase.Length; ++i) {

				bankRef.rosetta.registerString (bankRef.extra + bankRef.wisdom + bankRef.subWisdom + "_" + i, bankRef.phrase [i]);

			}

		}

		if (GUILayout.Button ("Update prefab")) {

			Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/StringBanks/StringBank(" + bankRef.extra + bankRef.wisdom + bankRef.subWisdom + ").prefab");
			PrefabUtility.ReplacePrefab (bankRef.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

		}

	}


}

#endif