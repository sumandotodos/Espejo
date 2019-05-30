using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableUsage {

	int[] rows;
	int round;
	int roundRemain;
	public int tableIndex;

	//public SituationChooser parent;

	//public TablaAuxiliar correspondences;

	public void initialize(int nRows) {
		rows = new int[nRows];
		for (int i = 0; i < nRows; ++i) {
			rows [i] = 0;
		}
		round = 0;
		roundRemain = nRows;
	}

	public void markRow(int row) {
		rows [row]++;
	}

	public int selectRow() {

		// find a free index
		int index = Random.Range (0, rows.Length);
		while (rows [index] != round) {
			index = Random.Range (0, rows.Length);
		}
			
		rows [index]++; // mark as used

		--roundRemain;

		if (roundRemain == 0) {
			roundRemain = rows.Length;
			round++;
		}

		return index;

	}

}
