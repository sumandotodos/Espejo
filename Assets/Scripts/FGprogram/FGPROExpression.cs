using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FGBasicTypes { Int, Float, Bool, String, Void };

public abstract class FGPROExpression  {

	public abstract int getValue_int ();
	public abstract bool getValue_bool ();
	public abstract string getValue_string ();
	public abstract float getValue_float ();

	protected FGBasicTypes type;

	public FGBasicTypes getType() {
		return type;
	}

	//public abstract void setValue (T newValue);
	//public abstract void refresh();

}
