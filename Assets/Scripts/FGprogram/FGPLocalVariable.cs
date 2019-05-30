using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGPLocalVariable : FGPExpression {

	private int value_int;
	private float value_float;
	private string value_string;
	private bool value_bool;

	public override int getValue_int() {
		return value_int;
	}
	public override bool getValue_bool() {
		return value_bool;
	}
	public override float getValue_float() {
		return value_float;
	}
	public override string getValue_string() {
		return value_string;
	}

	public override void setValue(int newValue) {
		value_int = newValue;
	}

	public override void setValue(bool newValue) {
		value_bool = newValue;
	}

	public override void setValue(string newValue) {
		value_string = newValue;
	}

	public override void setValue(float newValue) {
		value_float = newValue;
	}

	public override void setValue(FGPExpression expr) {
		switch(expr.getType()) {
		case FGBasicTypes.Int:
			value_int = expr.getValue_int ();
			break;
		case FGBasicTypes.Bool:
			value_bool = expr.getValue_bool ();
			break;
		case FGBasicTypes.String:
			value_string = expr.getValue_string ();
			break;
		case FGBasicTypes.Float:
			value_float = expr.getValue_float ();
			break;
		}

	}

	public FGPLocalVariable(int cValue) {
		value_int = cValue;
		type = FGBasicTypes.Int;
	}
	public FGPLocalVariable(bool bValue) {
		value_bool = bValue;
		type = FGBasicTypes.Bool;
	}
	public FGPLocalVariable(string sValue) {
		value_string = sValue;
		type = FGBasicTypes.String;
	}
	public FGPLocalVariable(float fValue) {
		value_float = fValue;
		type = FGBasicTypes.Float;
	}



}
