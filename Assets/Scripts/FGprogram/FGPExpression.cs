using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FGPExpression : FGPROExpression {

	public abstract void setValue (int newValue);
	public abstract void setValue (float newValue);
	public abstract void setValue (string newValue);
	public abstract void setValue (bool newValue);
	public abstract void setValue (FGPExpression newValue);

}
