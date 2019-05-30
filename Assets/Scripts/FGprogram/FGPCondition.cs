using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGPCondition  {

	FGPROExpression left;
	string comparator;
	FGPROExpression right;

	public FGPCondition(FGPROExpression e1, string comp, FGPROExpression e2) {
		left = e1;
		comparator = comp;
		right = e2;
	}

	public bool eval() {
		FGBasicTypes exprType = left.getType ();
		switch (exprType) {
		case FGBasicTypes.Int:
			int ivalue1, ivalue2;
			ivalue1 = left.getValue_int ();
			ivalue2 = right.getValue_int ();
			return (compareValues (ivalue1, ivalue2, comparator));
				
		case FGBasicTypes.Float:
			float fvalue1, fvalue2;
			fvalue1 = left.getValue_float ();
			fvalue2 = right.getValue_float ();
			return (compareValues (fvalue1, fvalue2, comparator));

		case FGBasicTypes.String:
			string svalue1, svalue2;
			svalue1 = left.getValue_string ();
			svalue2 = right.getValue_string ();
			return (compareValues (svalue1, svalue2, comparator));
				
		case FGBasicTypes.Bool:
			bool bvalue1, bvalue2;
			bvalue1 = left.getValue_bool ();
			bvalue2 = right.getValue_bool ();
			return (compareValues (bvalue1, bvalue2, comparator));
		}
		return false;
				
	}

	private bool compareValues(int v1, int v2, string comp) {

		if (comp.Equals (">")) {
			return v1 > v2;
		}
		if (comp.Equals ("<")) {
			return v1 < v2;
		}
		if (comp.Equals ("==")) {
			return v1 == v2;
		}
		if (comp.Equals ("!=")) {
			return v1 != v2;
		}
		if (comp.Equals (">=")) {
			return v1 >= v2;
		}
		if (comp.Equals ("<=")) {
			return v1 <= v2;
		}
		return false;

	}
	private bool compareValues(float v1, float v2, string comp) {

		if (comp.Equals (">")) {
			return v1 > v2;
		}
		if (comp.Equals ("<")) {
			return v1 < v2;
		}
		if (comp.Equals ("==")) {
			return v1 == v2;
		}
		if (comp.Equals ("!=")) {
			return v1 != v2;
		}
		if (comp.Equals (">=")) {
			return v1 >= v2;
		}
		if (comp.Equals ("<=")) {
			return v1 <= v2;
		}
		return false;

	}
	private bool compareValues(bool v1, bool v2, string comp) {


		if (comp.Equals ("==")) {
			return v1 == v2;
		}
		if (comp.Equals ("!=")) {
			return v1 != v2;
		}
		return false;

	}
	private bool compareValues(string v1, string v2, string comp) {


		if (comp.Equals ("==")) {
			return v1.Equals(v2);
		}
		if (comp.Equals ("!=")) {
			return v1.Equals(v2);
		}
		return false;

	}

}
