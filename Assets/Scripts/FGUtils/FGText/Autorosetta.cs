
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

public class Autorosetta : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void go() {
		
		StringBank[] allStringBanks = GameObject.FindObjectsOfType (typeof(StringBank)) as StringBank[];
		Rosetta rosetta = GameObject.Find ("Rosetta").GetComponent<Rosetta> ();

		for (int i = 0; i < allStringBanks.Length; ++i) {

			StringBank bank = (StringBank)(allStringBanks [i]);


			bank.rosetta = rosetta;

			for (int j = 0; j < bank.phrase.Length; ++j) {

				rosetta.registerString (bank.extra + bank.wisdom + bank.subWisdom + "_" + j, bank.phrase [j]);

			}


		}

		Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/Rosetta.prefab");
		PrefabUtility.ReplacePrefab (rosetta.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

	}
}

#endif