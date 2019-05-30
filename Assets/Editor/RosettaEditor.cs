
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Rosetta))]
public class RosettaEditor : Editor  {


		public override void OnInspectorGUI() {

			DrawDefaultInspector ();

			

			Rosetta rosRef = (Rosetta)target;

		if (rosRef.data.Count == 0) {
			rosRef.reset ();
		}

			RosettaHashDict currentTranslation = rosRef.data[rosRef.currentTranslationIndex];


		GUI.backgroundColor = Color.red;
		EditorGUILayout.LabelField ("Dict data: " + currentTranslation);
		EditorGUILayout.LabelField ("Dict data.data: " + currentTranslation.data);
		GUI.backgroundColor = Color.white;

			if (GUILayout.Button("Reset")) {
				rosRef.reset ();
			}

			EditorGUILayout.LabelField ("Number of translations loaded: " + rosRef.nTranslations);
			EditorGUILayout.LabelField ("Number of strings per translation: " + rosRef.nStrings);
			EditorGUILayout.LabelField ("First level hash size: " + rosRef.firstLevelHashSize);
			EditorGUILayout.LabelField ("Second level hash size: " + rosRef.secondLevelHashSize);
			EditorGUILayout.LabelField ("Current locale: " + currentTranslation.localeName);

			string[] translationList = rosRef.getLoadedTranslationsNames ();
			rosRef.currentTranslationIndex = EditorGUILayout.Popup (rosRef.currentTranslationIndex, translationList);
			rosRef.setLocale (rosRef.data [rosRef.currentTranslationIndex].localeName);

			rosRef.deletePrefix = GUILayout.TextField (rosRef.deletePrefix, GUILayout.Width(200));
			if (GUILayout.Button("Delete prefix", GUILayout.Width(200))) {

				rosRef.deleteStringsByPrefix ();


			}
			if (GUILayout.Button("Export to clipboard", GUILayout.Width(200))) {
				EditorGUIUtility.systemCopyBuffer = rosRef.translationToString (); 
			}

			if (GUILayout.Button("Import from clipboard", GUILayout.Width(200))) {
				AllTerrainParser parser = new AllTerrainParser (EditorGUIUtility.systemCopyBuffer + "%%%%%%%%%");

				// parse locale name
				parser.setParserMode (ParserMode.begin);
				parser.scanToChar (':');
				parser.setParserMode (ParserMode.end);
				parser.scanToNextValidChar ();
				parser.scanUntilValidChar ();
				parser.setParserMode (ParserMode.begin);
				parser.scanToNextValidChar ();
				string newLocaleName = parser.extract ();
				
				if (newLocaleName.Length == 0) // parse failed
					goto parseEnd;	

				RosettaHashDict targetDict = null;

				// See if locale already exists 
				int dictionaryIndex = 0;
				for (int loc = 0; loc < rosRef.data.Count; ++loc) {
				if (rosRef.data [loc].localeName.Equals (newLocaleName)) {
					targetDict = rosRef.data [loc];
					dictionaryIndex = loc;
				}
					continue;
				}


				// If it doesn't exist, add it
				if(targetDict == null) { targetDict = new RosettaHashDict ();
					targetDict.initialize (rosRef.firstLevelHashSize, rosRef.secondLevelHashSize);
					targetDict.localeName = newLocaleName;
					rosRef.data.Add(targetDict);
					dictionaryIndex = rosRef.data.Count-1;
				}
			
					string newKey="", newTranslation="";
					// start parsing key:translation pairs
					do {

						parser.scanToNextLine();
						parser.setParserMode(ParserMode.end);
						parser.scanToChar(':');
						parser.addOffset(-1);
						newKey = parser.extract();
						newKey = newKey.Trim();

						parser.scanToChar('\"');
						parser.scanToChar('\"');
						parser.setParserMode(ParserMode.begin);
						parser.scanToChar('\"'); // parse default string
						

						parser.setParserMode(ParserMode.end);
						parser.scanToChar('\"');
						parser.addOffset(-1);
						parser.setParserMode(ParserMode.begin);
						parser.scanToChar('\"');
						newTranslation = parser.extract();
						bool valid = true;
						if(newTranslation.Length==0) valid = false;
						for(int w = 0; w<newTranslation.Length; ++w) {
							if( (newTranslation[w]!='\n') && (newTranslation[w]!=' ') && (newTranslation[w]!='-') && (newTranslation[w]!='>')) {
								valid = false;
								continue;
							}
						}

						// if translation field is valid... 
						if(valid) {
							parser.setParserMode(ParserMode.end);
							parser.scanToChar('\"');
							parser.scanToChar('\"');
							parser.addOffset(-1);
							parser.setParserMode(ParserMode.begin);
							parser.scanToChar('\"');
							newTranslation = parser.extract();
							newKey = newKey.Trim();
							rosRef.registerString(newKey, newTranslation, dictionaryIndex);

						}
						else {

							parser.rewindEndHead();
							 // and loop over
							

						}

						parser.setParserMode(ParserMode.begin);

					} while(newTranslation.Length > 0);

			}

parseEnd:

			if (GUILayout.Button("Update prefab (Create instance first!)")) {
				
				
					Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/Rosetta.prefab");
					PrefabUtility.ReplacePrefab (rosRef.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
					
				
			}

			

		}




}


#endif