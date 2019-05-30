using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Phrase_t
{
	public int index;
	public int type;

	public Phrase_t(int i, int t)
	{
		index = i;
		type = t;
	}
}

[System.Serializable]
public class Player {

	public int score;
	public string login;
	public bool isPresent;
	public int mirrorDamage;

	public List<int> sciencePhrasesObtained;
	public List<int> mindPhrasesObtained;
	public List<Phrase_t> phrasesObtained;

	public Player() {
		score = 0;
		login = "";
		isPresent = false;
		sciencePhrasesObtained = new List<int> ();
		mindPhrasesObtained = new List<int> ();
		phrasesObtained = new List<Phrase_t> ();
		mirrorDamage = 0;
	}

}
