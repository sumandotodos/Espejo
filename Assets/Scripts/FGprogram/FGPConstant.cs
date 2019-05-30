using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FGPConstant : FGPROExpression {

	// basic types
	private int constantValue_int;
	private float constantValue_float;
	private string constantValue_string;
	private bool constantValue_bool;



	public override int getValue_int() {
		return constantValue_int;
	}

	public override float getValue_float() {
		return constantValue_float;
	}

	public override bool getValue_bool() {
		return constantValue_bool;
	}

	public override string getValue_string() {
		return constantValue_string;
	}
		

	public FGPConstant(int cValue) {
		constantValue_int = cValue;
		type = FGBasicTypes.Int;
	}

	public FGPConstant(float cValue) {
		constantValue_float = cValue;
		type = FGBasicTypes.Float;
	}

	public FGPConstant(bool cValue) {
		constantValue_bool = cValue;
		type = FGBasicTypes.Bool;
	}

	public FGPConstant(string cValue) {
		constantValue_string = cValue;
		type = FGBasicTypes.String;
	}

//	public override void refresh() {
//		// do nothing
//	}

}
