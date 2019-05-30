using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class FGPMethodCall : FGPROExpression {

	private Object targetObject;
	private string methodName;
	private List<string> paramsStr;
	private List<object> paramsObj;
	private object[] paramsArray;

	//private T constantValue;



	public override bool getValue_bool() {
		return (bool)getValue ();
	}

	public override string getValue_string() {
		return (string)getValue ();
	}

	public override float getValue_float() {
		return (float)getValue ();
	}

	public override int getValue_int() {
		return (int)getValue ();
	}

	public void getValue_void() {
		getValue ();
	}

	public object getValue() {

		MethodInfo m = targetObject.GetType ().GetMethod (methodName);
		ParameterInfo[] paramInfoList = m.GetParameters ();

		List<object> nativeParamList = new List<object> ();

		/*
		for (int k = 0; k < paramInfoList.Length; ++k) {
			System.Type t = paramInfoList [k].ParameterType;
			if (t.Name.Equals ("Int32")) {
				int iParam;
				//int.TryParse (paramsStr[k], out iParam); // parse into integer
				iParam = (int)paramsObj[k];
				nativeParamList.Add (iParam);
			} else if (t.Name.Equals ("Single")) {
				float fParam;
				//float.TryParse (paramsStr[k], out fParam);
				fParam = (float)paramsObj[k];
				nativeParamList.Add (fParam);
			} else if (t.Name.Equals ("String")) {
				nativeParamList.Add ((string)paramsObj[k]); // no parsing needed
			} else if (t.Name.Equals ("Boolean")) {
				bool bParam;
				//bool.TryParse (paramsStr[k], out bParam);
				bParam = (bool)paramsObj[k];
				nativeParamList.Add (bParam);

			}
		}*/


		return m.Invoke (targetObject, paramsArray); // nativeParamList.ToArray ());

	}


	public FGPMethodCall(Object obj, string mName, params object[] parameters) {
		//System.Type ttt = averquepasa.GetType ();
		//	constantValue = cValue;
		targetObject = obj;
		methodName = mName;
		paramsObj = new List<object> ();
		MethodInfo m = targetObject.GetType ().GetMethod (mName);

		type = systemTypeToFGType (m.ReturnType);
		//for (int i = 0; i < parameters.Length; ++i) {

		//	paramsObj.Add (parameters [i]);
		//}
		paramsArray = parameters;
	}
//
//	public static void letMeSeeShit(System.Ac llamada) {
//		System.Type ttt = llamada.GetType();
//		string nnn = ttt.Name;
//	}

	public FGBasicTypes systemTypeToFGType(System.Type t) {
		if (t.Name.Equals ("Int32")) {
			return FGBasicTypes.Int;
		} else if (t.Name.Equals ("Single")) {
			return FGBasicTypes.Float;
		} else if (t.Name.Equals ("String")) {
			return FGBasicTypes.String;
		} else if (t.Name.Equals ("Boolean")) {
			return FGBasicTypes.Bool;
		}
		return FGBasicTypes.Void;
	}


}
