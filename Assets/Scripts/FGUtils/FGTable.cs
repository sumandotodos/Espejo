using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum IterationMode { serial, random };

[System.Serializable]
public class EmparchisTable {

	public int N;
	public string SITUACION1;
	public string SITUACION2;
	public string EMOCIONES;
	public string EXPRESAR;
	public string TIPOSRESPUESTA;
	public string CIRCUNSTANCIAS;
	public string CREATIVIDAD;
	public string EQUILIBRIO;
	public int POSITIVA;

}

[System.Serializable]
public class EmparchisTableWrap {

	public List<EmparchisTable> data;

	public void fixEmociones() {
		foreach (EmparchisTable t in data) {
			t.EMOCIONES = t.EMOCIONES.Replace ('\n', ';');
//			if (t.EMOCIONES.Equals (""))
//				t.EMOCIONES = " ";
			t.SITUACION1 = t.SITUACION1.Replace ('\n', '#');
//			if (t.SITUACION1.Equals (""))
//				t.SITUACION1 = " ";
			t.SITUACION2 = t.SITUACION2.Replace ('\n', '#');
//			if (t.SITUACION2.Equals (""))
//				t.SITUACION2 = " ";
			t.EXPRESAR = t.EXPRESAR.Replace ('\n', '#');
//			if (t.EXPRESAR.Equals (""))
//				t.EXPRESAR = " ";
			t.TIPOSRESPUESTA = t.TIPOSRESPUESTA.Replace ('\n', '#');
//			if (t.TIPOSRESPUESTA.Equals (""))
//				t.TIPOSRESPUESTA = " ";
			t.CIRCUNSTANCIAS = t.CIRCUNSTANCIAS.Replace ('\n', '#');
//			if (t.CIRCUNSTANCIAS.Equals (""))
//				t.CIRCUNSTANCIAS = " ";
			t.CREATIVIDAD = t.CREATIVIDAD.Replace ('\n', '#');
//			if (t.CREATIVIDAD.Equals (""))
//				t.CREATIVIDAD = " ";
			t.EQUILIBRIO = t.EQUILIBRIO.Replace ('\n', '#');
//			if (t.EQUILIBRIO.Equals (""))
//				t.EQUILIBRIO = " ";
		}
	}

}

public class FGTable : MonoBehaviour {

	public const int TypeInteger = 0;
	public const int TypeString = 1;

	TableUsage tableUsage;

	public RosettaWrapper rosettaWrapper;

	public int cols;
	public int rows;

	public List<FGColumn> column;
	//public string[] columnName;

	public IterationMode mode;

	int serialIndex = 0;

	public FGTable() {
		column = new List<FGColumn> ();

		cols = 0;
		rows = 0;
	}

	void Start() {
		tableUsage = new TableUsage ();
		tableUsage.initialize (nRows());
	}

	public int getNextRowIndex () {

		int res;

		if (mode == IterationMode.serial) {

			res = serialIndex;
			serialIndex = (serialIndex + 1) % rows;

		} else {

			res = tableUsage.selectRow ();

		}
		return res;

	}

	public object getElement(int c, int r) {

		object res = 0;

		if (column [c].getType () == TypeInteger) {
			FGIntColumn intCol = (FGIntColumn)column [c];
			res = intCol.getRow (r);
		}
		else if (column [c].getType () == TypeString) {
			FGStringColumn strCol = (FGStringColumn)column [c];
			res = rosettaWrapper.rosetta.retrieveString (strCol.rosettaPrefixName, r);
		}
		return res;

	}

	public FGColumn getColumn(int c) {
		return column [c];
	}

	public int nColumns() {
		return column.Count;
	}

	public int nRows() {
		// we trust that all columns have the same length
		return column [0].nItems();
	}


	public string JSON2CRSV(string json) {

		string res = "";
		string header = "";
		bool captureHeader = true;

		int getNColumnsState = 0;
		int firstFieldLine = 0;
		int lastFieldLine = 0;

		string[] files = json.Split ('\n');
		for (int i = 0; i < files.Length; ++i) {
			string[] fields = files [i].TrimEnd(',').Split (':');
			if ((fields.Length < 2) && !header.Equals("")) {
				captureHeader = false;
			}
			if (fields.Length == 2) { // golly! a field!

				if (captureHeader)
					header += (fields [0].Replace("\"", "").Replace(" ", "").Replace("\t", "") + ":");

				if (getNColumnsState == 0) {
					getNColumnsState = 1;
					firstFieldLine = i;
				}

				// try integer value
				int value;
				if (int.TryParse (fields [1], out value)) {
					res += (value + "\n");
				} else {
					string pollitoLimpio = fields [1];//.Substring (2);
					pollitoLimpio = pollitoLimpio.TrimStart (' '); //pollitoLimpio.Substring (0, pollitoLimpio.Length - 1);
					pollitoLimpio = pollitoLimpio.TrimStart ('\"');
					pollitoLimpio = pollitoLimpio.TrimEnd ('\"');
					pollitoLimpio = pollitoLimpio.Replace ("\\n", ";");
					res += (pollitoLimpio + "\n");
				}
			} 

			else { // some superfluous shit
				if (getNColumnsState == 1) {
					getNColumnsState = 2;
					lastFieldLine = i;
				}
			}
		}

		header = header.Substring (0, header.Length - 1);
		header += "\n";

		//return (lastFieldLine - firstFieldLine) + "\n" + res.TrimEnd('\n');
		return header + res.TrimEnd('\n');

	}

	public string exportJSON() {

		bool hasColumnNames = false;
		for (int i = 0; i < column.Count; ++i) {
			if (!column [i].columnName.Equals ("")) {
				hasColumnNames = true;
				break;
			}
		}

		string res = "[\n";
		for (int r = 0; r < nRows (); ++r) {
			res += "\t{\n";
			for (int i = 0; i < column.Count; ++i) {
				if (hasColumnNames) {
					res += "\t\t\"" + column [i].columnName + "\": ";
				} else {
					res += "\t\t\"COLUMN" + i + "\": ";
				}
				if (column [i].getType () == FGTable.TypeInteger) {
					int data = (int)(getElement (i, r));
					res += data;
				} else {
					string data = (string)(getElement (i, r));
					res += ("\"" + data + "\"");
				}
				if (i < (column.Count - 1)) {
					res += ",\n";
				} else {
					res += "\n";
				}
			}
			if (r < (nRows () - 1)) {
				res += "\t},\n";
			} else {
				res += "\t}\n";
			}
		}

		res+= "]\n";
		return res;

	}

	public string exportCRSV() {

		bool hasColumnNames = false;
		for (int i = 0; i < column.Count; ++i) {
			if (!column [i].columnName.Equals ("")) {
				hasColumnNames = true;
				break;
			}
		}

		string res = "";
		if (hasColumnNames) {
			res += column [0].columnName;
			for (int i = 1; i < column.Count; ++i) {
				res += (":" + column [i].columnName);
			}
			res += "\n";
		} else {
			res = column.Count + "\n";
		}

		for (int r = 0; r < nRows (); ++r) {
			for (int i = 0; i < column.Count; ++i) {
				if (column [i].getType () == FGTable.TypeInteger) {
					int data = (int)(getElement (i, r));
					res += (data + "\n");
				} else {
					string data = (string)(getElement (i, r));
					res += (data + "\n");
				}
			}
		}

		res.TrimEnd ('\n');
		return res;

	}

	/*
	 * 
	 * Specific version for Emparchis
	 *  
	 */

	public string importJSONEmparchis(string c) {

		if (c.StartsWith ("[")) { // wrap!
			c = "{ \"data\": " + c + "}";
		}

		EmparchisTableWrap tableData = JsonUtility.FromJson<EmparchisTableWrap> (c);

		tableData.fixEmociones ();

		string crsvRep = "9\n"; // 10 columns

		for(int i = 0; i < tableData.data.Count; ++i) {
		//foreach (EmparchisTable t in tableData.data) {
			EmparchisTable t = tableData.data[i];
			crsvRep += (t.SITUACION1 + "\n");
			crsvRep += (t.SITUACION2 + "\n");
			crsvRep += (t.EMOCIONES + "\n");
			crsvRep += (t.EXPRESAR + "\n");
			crsvRep += (t.TIPOSRESPUESTA + "\n");
			crsvRep += (t.CIRCUNSTANCIAS + "\n");
			crsvRep += (t.CREATIVIDAD + "\n");
			crsvRep += (t.EQUILIBRIO + "\n");
			if (i < (tableData.data.Count - 1))
				crsvRep += (t.POSITIVA + "\n");
			else
				crsvRep += (t.POSITIVA);
		}

		return crsvRep;


	}

	public void importCRSV(string c) {
		
		int nColumns = 1;
		int offset = 1;
		string[] colNames = null;

		string[] files = c.Split ('\n');
		int n;
		if (int.TryParse (files [0], out n)) {
			nColumns = n;
			//offset = 1;
		} else {
			colNames = files [0].Split (':');
			nColumns = colNames.Length;
			//offset = 1;
		}

		for (int k = 0; k < nColumns; ++k) { // process each column

			GameObject newCol = new GameObject ();
			newCol.name = "Column" + k;
			newCol.transform.SetParent (this.transform);
			// test type of data
			int intTest;
			int type;
			if (int.TryParse (files [offset + k], out intTest)) {
				newCol.AddComponent<FGIntColumn> ();
				if (colNames == null) {
					newCol.GetComponent<FGIntColumn> ().columnName = "COLUMN" + k;
				} else {
					newCol.GetComponent<FGIntColumn> ().columnName = colNames [k];
				}
				type = FGTable.TypeInteger;
			} else {
				newCol.AddComponent<FGStringColumn> ();
				newCol.GetComponent<FGStringColumn> ().rosettaPrefixName = this.name + "_" + k;
				if (colNames == null) {
					newCol.GetComponent<FGStringColumn> ().columnName = "COLUMN" + k;
				} else {
					newCol.GetComponent<FGStringColumn> ().columnName = colNames [k];
				}
				type = FGTable.TypeString;
			}
			int nRow = 0;
			for (int i = offset + k; i < files.Length; i += nColumns) {

				//if (!files [i].Equals ("")) {
					if (type == FGTable.TypeInteger) {
						int newData;
						int.TryParse (files [i], out newData);
						newCol.GetComponent<FGIntColumn> ().addData (newData);
					} else if (type == FGTable.TypeString) {
						rosettaWrapper.rosetta.registerString (this.name + "_" + k + "_" + nRow, files [i]);
						++nRow;
					}
				//}


			}

			if (type == FGTable.TypeString) {
				newCol.GetComponent<FGStringColumn> ().length = nRow;
			}
			column.Add (newCol.GetComponent<FGColumn> ());

		}
			

	}

	public void reset() {
		column = new List<FGColumn> ();
		cols = 0;
		rows = 0;
		FGColumn[] colsToDelete;
		colsToDelete = this.GetComponentsInChildren<FGColumn> ();
		for(int i = 0; i < colsToDelete.Length; ++i) {
			DestroyImmediate(colsToDelete[i].gameObject);
		}

	}

}
