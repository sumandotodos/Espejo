using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Wrapper class for serialization */

[System.Serializable]
public class ListRosettaDictElement3D {

	public List<ListRosettaDictElement2D> theList;

	public ListRosettaDictElement3D () {

		theList = new List<ListRosettaDictElement2D>();

	}

}