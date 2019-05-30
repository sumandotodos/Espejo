using UnityEngine;
using System.Collections;

public class TweenTransforms {

	public static float linear(float origin, float current, float dest) {
		return current;
	}
		
	public static float cubicOut(float origin, float current, float dest) {
		float r = 0.0f;
		if (dest == origin)
			return current;
		float t = (current - origin) / (dest - origin);
		r = (t * t * t + -3 * t * t + 3*t);
		return origin + r * (dest - origin);
	}

	public static float tanh(float origin, float current, float dest) {
		float r = 0.0f;
		if (dest == origin)
			return current;
		if (current == origin)
			r = 0.0f;
		else if (current == dest)
			r = 1.0f;
		else {
			float t = (current - origin) / (dest - origin); // 0..1 space parameter
			r = 0.5f + ((Mathf.Exp ((-2.5f) + t * 5.0f) - Mathf.Exp (-((-2.5f) + t * 5.0f))) / (Mathf.Exp ((-2.5f) + t * 5.0f) + Mathf.Exp (-((-2.5f) + t * 5.0f)))) / 2.0f; // 0..1 space result
		}
		return origin + r*(dest-origin);

	}

}

public abstract class SoftVariable {

	float target;
	float t;
	protected float speed;
	protected System.Func<float, float, float, float> transformation;
	public void setSpeed(float newSpeed) {
		speed = newSpeed;
	}
}

public class SoftFloat:SoftVariable {

	public float prevValue;
	private float value;
	public float linSpaceTarget;
	public float linSpaceValue;
	public float linSpaceOrigin;
	public float t;

	public SoftFloat() {
		prevValue = 0.0f;
		value = 0.0f;
		linSpaceValue = 0.0f;
		linSpaceTarget = 0.0f;
		linSpaceOrigin = 0.0f;
		transformation = TweenTransforms.linear;
	}

	public SoftFloat(float initial) {
		prevValue = initial;
		value = initial;
		linSpaceValue = initial;
		linSpaceTarget = initial;
		linSpaceOrigin = initial;
		transformation = TweenTransforms.linear;
	}

	public void setTransformation(System.Func<float, float, float, float> transformFunc) {
		transformation = transformFunc;
	}

	public float getValue() {
		if (linSpaceValue == linSpaceTarget)
			return linSpaceTarget;
		if (linSpaceValue == linSpaceOrigin)
			return linSpaceOrigin;
		value = transformation (linSpaceOrigin, linSpaceValue, linSpaceTarget);
		return value;
	}

	public void setValueImmediate(float newValue) {
		prevValue = newValue;
		value = newValue;
		linSpaceTarget = newValue;
		linSpaceOrigin = newValue;
		linSpaceValue = newValue;
	}

	public void setValue(float newFloat) {
		prevValue = value;
		value = newFloat;
		linSpaceTarget = value;
		linSpaceValue = prevValue;
		linSpaceOrigin = prevValue;
	}

	public bool update() {

		if (linSpaceValue > linSpaceTarget) {
			linSpaceValue -= speed * Time.deltaTime;
			if (linSpaceValue < linSpaceTarget) {
				linSpaceValue = linSpaceTarget;
				return false;
			}
			return true;
		} else if (linSpaceValue < linSpaceTarget) {
			linSpaceValue += speed * Time.deltaTime;
			if (linSpaceValue > linSpaceTarget) {
				linSpaceValue = linSpaceTarget;
				return false;
			}
			return true;
		} else
			return false;

	}

}
[System.Serializable]
public class BootStrapData {

	public string loginServer;
	public int loginServerPort;
	public string socketServer;
	public int socketServerPort;
	public string commandServer;
	public int commandServerPort;

	public BootStrapData(string ls, int lsp, string ss, int ssp, string cs, int csp) {
		loginServer = ls;
		loginServerPort = lsp;
		socketServer = ss;
		socketServerPort = ssp;
		commandServer = cs;
		commandServerPort = csp;
	}
}

public class FGUtils : MonoBehaviour {

	/* constants */

	public const string flygamesLoginCheck = "https://apps.flygames.org:9090";
	public const string flygamesSSLAuthHost = "apps.flygames.org";
	public const string getCountryListScript = "/listCountries";
	public const string getLocalitiesListScript = "/listLocalitiesByCountry";
	public const string getOrganizationsListScript = "/listOrganizationByLocCoun";
	public const string getClassroomsListScript = "/listClassroomsByOrgLocCoun";
	public const string getDebateDB = "/retrieveDebateDB";
	public const string getUserNickname = "/getUserNickname";
	public const string setUserNickname = "/setUserNickname";
	public const string RecoveryScript = "/requestNewPassword";
	public const string CheckUserScript = "/checkUser";
	public const string NewUserScript = "/newUser";
	public const string GetFreshRoomID = "/login/nextRoomID";
	public const string ReleaseRoomID = "/clearRoomID";
	public const string GameRelayServer = "apps.flygames.org"; // primary Linode
	public const int GameRelayPort = 13072; // Generic relay server
	public const string localGamePrefix = "EspLite";

	public const string BootstrapURL = "http://apps.flygames.org/bootstrap";

	public const string fallbackLoginServer = "https://apps.flygames.org";
	public const int fallbackLoginServerPort = 25;
	public const string fallbackSocketServer = "apps.flygames.org";
	public const int fallbackSocketServerPort = 443;
	public const string fallbackCommandServer = "apps.flygames.org";
	public const int fallbackCommandServerPort = 110;

	public const string appsPSKSecret = "g2T21X48tJ21pqx7571ad90";

	public const int build = 23;

	public const int socketPort = 993;

	public const int MsgRepeatedLogin = 5;
	public const int MsgIncompatVersion = 4;

	public const string compatibilityCode = "001"; // deprecated

	public const float delta = 0.01f;

	public const int facesRandomSeed = 11131979;

	public const float virtualWidth = 1920.0f;


	public static void bootstrapServers(NetworkController agent) {

		WWW www = new WWW (BootstrapURL);
		while (!www.isDone) {
			// oh, no!
		}
		string jsonRep = www.text;
		if (jsonRep.Equals ("")) {
			agent.bootstrapData = new BootStrapData (fallbackLoginServer,
				fallbackLoginServerPort,
				fallbackSocketServer,
				fallbackSocketServerPort,
				fallbackCommandServer,
				fallbackCommandServerPort);
		}
		else agent.bootstrapData = JsonUtility.FromJson<BootStrapData> (jsonRep);


	}


	// WARNING: not exhaustive
	public static bool isValideMail(string email) {

		string[] at = email.Split ('@');
		if (at.Length != 2)
			return false;
		string[] dots = email.Split ('.');
		if (dots.Length < 2)
			return false;
		return true;

	}

	public static Vector2 physicalToVirtualCoordinates(Vector2 phys) {

		Vector2 res = new Vector2 ();
		res.x = phys.x * (virtualWidth / Screen.width) - virtualWidth / 2.0f;
		res.y = phys.y * (virtualWidth / Screen.width) - (virtualWidth * Screen.height/Screen.width)/2.0f;

		return res;

	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public static int pseudoRandom(int index, int max) {
		Random.InitState (facesRandomSeed + index);
		return Random.Range (0, max);

	}

	public void mecagoentodo() {

	}

	public static char decToHexChar(int d) {
		switch (d) {
		case 0:
			return '0';
		case 1:
			return '1';
		case 2:
			return '2';
		case 3:
			return '3';
		case 4:
			return '4';
		case 5:
			return '5';
		case 6:
			return '6';
		case 7:
			return '7';
		case 8:
			return '8';
		case 9:
			return '9';
		case 10:
			return 'A';
		case 11:
			return 'B';
		case 12:
			return 'C';
		case 13:
			return 'D';
		case 14:
			return 'E';
		case 15:
			return 'F';
		}
		return '0';
	}

	public static string valueToHexstring(float v) {

		int iVal = (int)(v*255.0f);

		int lo = iVal & 15;
		int hi = (iVal >> 4) & 15;

		return "" + decToHexChar (hi) + decToHexChar (lo);

	}

	public static string chopSpaces(string s) {
		string[] strs = s.Split (' ');
		string res = strs [0];
		for (int i = 1; i < strs.Length; ++i) {
			res += "\n" + strs [i];
		}
		return res;
	}


	//public static float updateSoftVariable(float val, float target

	/*
	public static bool updateSoftVariable(ref float val, float target, float speed, System.Func<float, float> transf) {

	}

	public static bool updateSoftVariable(ref float val, float target, float speed) {

		bool hasChanged = false;

		if (val < (target-delta)) {
			val += speed * Time.deltaTime;
			hasChanged = true;
			if (val > target)
				val = target;
		}

		if (val > (target+delta)) {
			val -= speed * Time.deltaTime;
			hasChanged = true;
			if (val < target)
				val = target;
		}

		if (!hasChanged)
			val = target;

		return hasChanged;

	}*/



	/*public static void queueMessage(string msg) {

		string uuid = SystemInfo.deviceUniqueIdentifier;
		GameObject MailQueueGO = new GameObject ();
		MailQueueGO.name = "MailQueueAgent";
		MailQueueGO.AddComponent<QueueMailAgent> ().initialize (uuid, msg);
		DontDestroyOnLoad (MailQueueGO);


	}*/

	public static int pseudoRandom(int input) {

		int shiftBits = 7;
		int hash = 5381;
		int c;
		int i = 0;

		for (i = 0; i < (input+3); ++i) {
			c = i*3;
			hash = ((hash << shiftBits) + hash) + c; /* hash * 33 + c */
		}

		if (hash < 0)
			return -hash;
		return hash;

	}

}
