using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FGColumn : MonoBehaviour {

	public string columnName;
	public abstract int nItems();
	public abstract int getType ();

}
