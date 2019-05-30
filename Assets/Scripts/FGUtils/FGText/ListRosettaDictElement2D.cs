using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Wrapper class for serialization */

[System.Serializable]
public class ListRosettaDictElement2D {

	public List<ListRosettaDictElement1D> theList;

	public ListRosettaDictElement2D () {

		theList = new List<ListRosettaDictElement1D>();

	}

}