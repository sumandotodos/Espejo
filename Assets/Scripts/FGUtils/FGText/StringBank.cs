using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class StringBank : MonoBehaviour {
	
	[HideInInspector]
	public string subWisdom;
	public Rosetta rosetta;
	[HideInInspector]
	public string wisdom;
	public string extra;
	[HideInInspector]
	public bool randomYield;
	int prevRandom;


	public string[] phrase;
	public int[] color;

	//public int numberOfPhrases;
	int nextPhrase;

	// Use this for initialization
	void Start () {

		reset ();
	
	}

	public void reset() {

		nextPhrase = -1;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string getNextString() {

		if (randomYield) {
			nextPhrase = Random.Range (0, phrase.Length - 1);
			if (phrase.Length > 1) {
				if (nextPhrase == prevRandom)
					nextPhrase = (nextPhrase + 1) % phrase.Length;
			}
			prevRandom = nextPhrase;
		}
		else
			nextPhrase = (nextPhrase + 1) % phrase.Length;

		return rosetta.retrieveString (extra + wisdom + subWisdom + "_" + nextPhrase);


	}

	public string getNextStringId() {

		if (randomYield) {
			nextPhrase = Random.Range (0, phrase.Length - 1);
			if (phrase.Length > 1) {
				if (nextPhrase == prevRandom)
					nextPhrase = (nextPhrase + 1) % phrase.Length;
			}
			prevRandom = nextPhrase;
		}
		else
			nextPhrase = (nextPhrase + 1) % phrase.Length;

		return extra + wisdom + subWisdom + "_" + nextPhrase;


	}

	public string getStringId(int index) {

		if (phrase.Length > index)
			return extra + wisdom + subWisdom + "_" + index;
		else
			return "";

	}

	public string getString(int index) {

		if (phrase.Length > index)
			return rosetta.retrieveString (extra + wisdom + subWisdom + "_" + index);
		else
			return "";

	}

	public int nItems() {

		return phrase.Length;

	}
}
