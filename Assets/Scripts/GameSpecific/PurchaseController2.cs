using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseController2 : MonoBehaviour {

	public ControllerHub controllerHub;

	public void addCredits(int creds) {
		Debug.Log ("PC2 add credits: " + creds);
		controllerHub.titleController.accountCredits += creds;
		controllerHub.titleController.creditsHUD.text = "Créditos: " + controllerHub.titleController.accountCredits;
		controllerHub.titleController.IAPCanvas.SetActive (false);
		WWWForm wwwform = new WWWForm();
		wwwform.AddField ("email", controllerHub.gameController.gameState.localEMail);
		wwwform.AddField ("psk", FGUtils.appsPSKSecret);
		wwwform.AddField ("amount", "" + creds);
		wwwform.AddField ("app", "EspLite");
		new WWW (controllerHub.networkController.bootstrapData.loginServer + ":" +
			controllerHub.networkController.bootstrapData.loginServerPort + "/addCredits", wwwform);
		controllerHub.titleController.cancelIAP ();
		
	}


	public void failTransaction() {
		Debug.Log ("Transaction failed for some reason");
		controllerHub.titleController.cancelIAP ();
	}
}
