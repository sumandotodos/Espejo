using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGIntColumn : FGColumn {

	public List<int> data;

	public FGIntColumn() {
		data = new List<int> ();
	}

	public object getRow(int n) {
		return data [n];
	}

	public void addData(int newData) {
		data.Add (newData);
	}

	public override int nItems() {
		return data.Count;
	}

	public override int getType() {
		return FGTable.TypeInteger;
	}

}
