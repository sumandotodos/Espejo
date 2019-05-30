using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RosettaHashDict {

	[SerializeField]
	public int firstLevelHashSize;
	[SerializeField]
	public int secondLevelHashSize;
	[SerializeField]
	public string localeName;

	[SerializeField]
	public ListRosettaDictElement3D data;



	public void initialize (int fls, int sls) {

		firstLevelHashSize = fls;
		secondLevelHashSize = sls;
		ListRosettaDictElement2D new2DList;
		ListRosettaDictElement1D new1DList;

		data = new ListRosettaDictElement3D();


			for(int i = 0; i<fls; ++i) {

			new2DList = new ListRosettaDictElement2D ();
										
				for (int j = 0; j < sls; ++j) {

					new1DList = new ListRosettaDictElement1D ();

					new2DList.theList.Add (new1DList);
					
				
				}

			data.theList.Add (new2DList);

		}

	
	}
	

}
