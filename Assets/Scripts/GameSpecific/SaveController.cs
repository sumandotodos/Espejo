using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class TipSaveData {
	public List<string> dismissedTips;
}

[System.Serializable]
public class LoginSaveData {

	public string login;
	public string passwd;
	public string email;

}

[System.Serializable]
public class ObtainedKeysSaveData {

	public List<LinkKey> scienceList;
	//public List<LinkKey> mindList;

}

[System.Serializable]
public class QuickSaveInfo {

	public int numberOfPlayers; // 0 means NO quicksaveinfo
	public string roomId;
	public string randomChallenge;
	public string login;
	public string master;
	public string datetime;
	public string playcode;

	public QuickSaveInfo() {
		playcode = "";
		datetime = "";
		master = "";
		login = "";
		randomChallenge = "";
		roomId = "";
	}

}

public class SaveController : MonoBehaviour {

	public ControllerHub controllerHub;

	public void loadObtainedKeys() {

		if (File.Exists (Application.persistentDataPath + "/save001.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save001.dat", FileMode.Open);
			ObtainedKeysSaveData data = (ObtainedKeysSaveData)formatter.Deserialize (file);
			controllerHub.gameController.obtainedKeys [0] = data.scienceList;
			//controllerHub.gameController.obtainedKeys [GameController.Mind] = data.mindList;
			file.Close ();

		} else {
			controllerHub.gameController.obtainedKeys [0] = new List<LinkKey> ();
			//controllerHub.gameController.obtainedKeys [GameController.Mind] = new List<LinkKey> ();
		}

	}

	public void loadTipData() {

		if (File.Exists (Application.persistentDataPath + "/tipdata.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/tipdata.dat", FileMode.Open);
			TipSaveData data = (TipSaveData)formatter.Deserialize (file);
			controllerHub.gameController.tipSaveData = data;
			file.Close ();

		} else {

			controllerHub.gameController.tipSaveData = new TipSaveData ();
			controllerHub.gameController.tipSaveData.dismissedTips = new List<string> ();

		}


	}

	public void loadLoginData() {
		
		if (File.Exists (Application.persistentDataPath + "/save000.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save000.dat", FileMode.Open);
			LoginSaveData data = (LoginSaveData)formatter.Deserialize (file);
			controllerHub.gameController.gameState.localLogin = data.login;
			controllerHub.gameController.gameState.localEMail = data.email;
			controllerHub.gameController.gameState.localPasswd = data.passwd;
			file.Close ();


		} else {
			controllerHub.gameController.gameState.localLogin = "";
			controllerHub.gameController.gameState.localEMail = "";
			controllerHub.gameController.gameState.localPasswd = "";
		}


	}

	public void saveLoginData() {

//		if (controllerHub.gameController.gameState.localLogin.Equals (""))
//			return;

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/save000.dat", FileMode.Create);

		LoginSaveData data = new LoginSaveData ();
		data.login = controllerHub.gameController.gameState.localLogin;
		data.email = controllerHub.gameController.gameState.localEMail;
		data.passwd = controllerHub.gameController.gameState.localPasswd;

		formatter.Serialize (file, data);
		file.Close ();

	}

	public void saveTipData() {

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/tipdata.dat", FileMode.Create);
	
		formatter.Serialize (file, controllerHub.gameController.tipSaveData);
		file.Close ();

	}

	public void saveObtainedKeys() {

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/save001.dat", FileMode.Create);

		ObtainedKeysSaveData data = new ObtainedKeysSaveData ();
		data.scienceList = controllerHub.gameController.obtainedKeys [GameController.Science];
		//data.mindList = controllerHub.gameController.obtainedKeys [GameController.Mind];

		formatter.Serialize (file, data);
		file.Close ();

	}



	public void resetQuickSaveInfo() {

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/quicksaveinfo.dat", FileMode.Create);

		QuickSaveInfo quickSaveInfo = new QuickSaveInfo ();
		quickSaveInfo.numberOfPlayers = 0;
		quickSaveInfo.roomId = "";
		quickSaveInfo.randomChallenge = "";
		quickSaveInfo.datetime = "";
		quickSaveInfo.master = "";
		quickSaveInfo.login = "";
		quickSaveInfo.playcode = controllerHub.gameController.quickSaveInfo.playcode;


		formatter.Serialize (file, quickSaveInfo);
		file.Close ();

	}

	public bool checkQuickSaveInfo() {

		if (File.Exists (Application.persistentDataPath + "/quicksaveinfo.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/quicksaveinfo.dat", FileMode.Open);
			QuickSaveInfo data = (QuickSaveInfo)formatter.Deserialize (file);
			if (data.playcode == null)
				data.playcode = "";
			controllerHub.gameController.quickSaveInfo = data;
			controllerHub.gameController.gameState.masterLogin = data.master;
			controllerHub.gameController.gameState.localLogin = data.login;
			file.Close ();
			return (controllerHub.gameController.quickSaveInfo.numberOfPlayers != 0);

		} else {
			controllerHub.gameController.quickSaveInfo = new QuickSaveInfo ();
			return false;
		}

	}

	public void saveQuickSaveInfo() {

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/quicksaveinfo.dat", FileMode.Create);

		QuickSaveInfo quickSaveInfo = new QuickSaveInfo ();
		quickSaveInfo.numberOfPlayers = controllerHub.gameController.gameState.nPlayers;
		quickSaveInfo.roomId = controllerHub.gameController.gameState.roomID;
		quickSaveInfo.randomChallenge = controllerHub.gameController.quickSaveInfo.randomChallenge;
		quickSaveInfo.datetime = controllerHub.gameController.quickSaveInfo.datetime;
		quickSaveInfo.login = controllerHub.gameController.gameState.localLogin;
		quickSaveInfo.master = controllerHub.gameController.gameState.masterLogin;
		quickSaveInfo.playcode = controllerHub.gameController.quickSaveInfo.playcode;

		formatter.Serialize (file, quickSaveInfo);
		file.Close ();


	}

	public bool loadQuickSaveData() {

		if (File.Exists (Application.persistentDataPath + "/quicksavedata.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/quicksavedata.dat", FileMode.Open);
			GameState data = (GameState)formatter.Deserialize (file);

			controllerHub.gameController.gameState = data;
			controllerHub.gameController.bookNoPlay = 0; // just in case
			controllerHub.gameController.gameState.wordGameFirstToWin = -1; // just in case

			file.Close ();
			return true;

		} else
			return false;

	}

	public void saveQuickSaveData() {

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/quicksavedata.dat", FileMode.Create);

		GameState quickSaveData = new GameState ();

		quickSaveData = controllerHub.gameController.gameState;

		formatter.Serialize (file, quickSaveData);
		file.Close ();

	}


}
