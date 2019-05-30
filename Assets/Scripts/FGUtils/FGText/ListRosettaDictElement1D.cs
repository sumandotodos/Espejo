using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Wrapper class for serialization */

[System.Serializable]
public class ListRosettaDictElement1D {

	public List<RosettaDictElement> theList;

	public ListRosettaDictElement1D () {

		theList = new List<RosettaDictElement>();

	}

}