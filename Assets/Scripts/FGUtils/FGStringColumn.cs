using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGStringColumn : FGColumn {

	public string rosettaPrefixName;

	public int length;


	public override int nItems() {
		return length;
	}

	public override int getType() {
		return FGTable.TypeString;
	}


}
