using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : FGProgram, ButtonPressListener {

	public InputField playcodeInput;

	// SBName: MenuItems

	const int INICIARSES = 0;
	const int CREARCUENT = 1;
	const int CORREOELEC = 2;
	const int CONTRASENA = 3;
	const int ENVIAR 	 = 4;
	const int REESTABLEC = 5;
	const int REPITECONT = 6;

	public UIScaleFader mascotScaler;

	public RosettaWrapper rosettaWrapper;

	public ControllerHub controllerHub;
	public UIFader coverFader;

	public RawImage backButtonImage;
	public GameObject instructionsButton;
	public UIScaleFader instructionsPanel;
	public string PDFLink;
	public string VideoLink;

	// local connections
	public GameObject rootMenu;
	public GameObject loginMenu;
	public GameObject newGameMenu;
	public GameObject newGameMenuAlt;
	public GameObject justLogoutMenu;
	public GameObject createAccountMenu;
	public GameObject passwordRecoveryMenu;
	public UIScaleFader confirmDeleteSessionMenu;

	public UIScaleFader noMoarCreditsPanel;
	public UIScaleFader noMoarMagicPanel;

	public GameObject buyButton;
	public Text creditsHUD;
	public GameObject IAPCanvas;

	public InputField LoginMenu_usermailText;
	public InputField LoginMenu_passwordText;
	public InputField newMagicInput;

	public InputField NewAccount_usermailText;
	public InputField NewAccount_pass1Text;
	public InputField NewAccount_pass2Text;
	public InputField NewAccount_magicText;

	public UITextStimulus loginNotValid;
	public UITextStimulus serverUnreachableStim;

	public UITextStimulus newUserNotValid;
	public UITextStimulus newUserPassNoMatch;

	public Text currentLoginText;
	public Text currentLoginTextAlt;
	public Text currentLoginQuis;

	public int accountCredits;

	public GameObject taccanvas;

	public void buttonPress() {
		goTo ("Gallery");
	}

	public void readLoginMenuUserMail() {
		controllerHub.gameController.gameState.localLogin = LoginMenu_usermailText.text;
	}
	public void readLoginMenuPassword() {
		controllerHub.gameController.gameState.localPasswd = LoginMenu_passwordText.text;
	}
	public string loginMenuUserMail() {
		return controllerHub.gameController.gameState.localLogin;
	}
	public string loginMenuPassword() {
		return controllerHub.gameController.gameState.localPasswd;
	}

	public void fadeCoverIn() {
		coverFader.fadeToTransparent ();
	}

	public void fadeCoverOut(FGProgram waiter) {
		coverFader.fadeToOpaqueTask (waiter);
	}

	public void copyLoginToNetworkAgent() {
		controllerHub.networkController.localUserLogin = controllerHub.gameController.gameState.localLogin;
	}
		


	public void setBackButtonEnabled(bool en, int when) {
		Debug.Log ("Set Back button: " + en + ", when: " + when);
		backButtonImage.enabled = en;
	}

	public void checkAppUser() {

		controllerHub.networkController.checkAppUser (controllerHub.gameController.gameState.localEMail, controllerHub.gameController.gameState.localPasswd);

	}

	//aux
	public void loggedOutSave() {
		controllerHub.gameController.gameState.localEMail = "";
		controllerHub.gameController.gameState.localLogin = "";
		controllerHub.saveController.saveLoginData ();
		Debug.Log ("loggedOutSave");
	}


	public void loginDataToGameController() {

		controllerHub.gameController.gameState.localEMail = LoginMenu_usermailText.text;
		controllerHub.gameController.gameState.localPasswd = LoginMenu_passwordText.text;
		updateLoginText ();

	}

	public void updateLoginText() {
		currentLoginText.text = controllerHub.gameController.gameState.localEMail;
		currentLoginTextAlt.text = controllerHub.gameController.gameState.localEMail;
	}

	const int playcodeYear = 2018;
	const int playcodeMonth = 4;
	const int playcodeDay = 15;

	const string CorrectPlayCode = "sabio56";

	bool freePlay = true;

	// yes, two things, I'm sorry
	public void checkQuickSaveInfoAndChooseNewGameMenu() {



		if (controllerHub.saveController.checkQuickSaveInfo ()) {
			newGameMenuAlt.SetActive (true);
			instructionsButton.SetActive (true);
		} else {
			newGameMenu.SetActive (true);
			instructionsButton.SetActive (true);
		}
		System.DateTime dt = System.DateTime.Now;
		System.DateTime pcdt = new System.DateTime (playcodeYear, playcodeMonth, playcodeDay, 0, 0, 0);
		if (dt.CompareTo (pcdt) <= 0) {
			playcodeInput.text = CorrectPlayCode;
			playcodeInput.gameObject.SetActive (false);
			freePlay = true;
		} else {
			playcodeInput.text = controllerHub.gameController.quickSaveInfo.playcode;
            //playcodeInput.gameObject.SetActive (true);
            //freePlay = false;
            freePlay = true;
		}

	}

	string whichProgram = "";

	/*public void postCheckAppUser() {

		//programIf ("userOK", "", "true", "==", controllerHub.networkController, "wwwResult");

		int userUUID;
		int.TryParse (controllerHub.networkController.wwwResult (), out userUUID);
		if (userUUID > -1) {
			goTo ("userOK");
		}
		controllerHub.gameController.gameState.localLogin = "" + userUUID;

	}*/
	//aux
	public void postCheckAppUser() {

		//programIf ("userOK", "", "true", "==", controllerHub.networkController, "wwwResult");

		string[] field = controllerHub.networkController.wwwResult ().Split (':');

		if (field.Length == 2) {

			int credits;
			int.TryParse (field [1], out credits);
			accountCredits = credits;
			updateCreditsHUD ();
			if (accountCredits == -2) { // special meaning: magic code stopped working
				newMagicInput.text = "";
				//noMoarMagicPanel.setEasyType(EaseType.boingOutMore);
				noMoarMagicPanel.scaleOut ();
				currentLoginQuis.text = controllerHub.gameController.gameState.localEMail;
				justLogoutMenu.SetActive (true);
				controllerHub.saveController.saveLoginData();
				goTo("AwaitingForCorrectMagic");
				return;
			}

			int userUUID;
			int.TryParse (field[0], out userUUID);
			if (userUUID > -1) {
				goTo ("userOK");
				setBackButtonEnabled (true, 11);
			} else {
				accountCredits = -100;
				updateCreditsHUD ();
			}

			controllerHub.gameController.gameState.localLogin = "" + userUUID;
		} 

	}

	//aux
	public void initialize() {
		buyButton.GetComponent<Button> ().interactable = false;
		controllerHub.saveController.loadTipData ();
	}
	//aux
	public void clearPassword() {
		LoginMenu_passwordText.text = "";
	}

	void Start () 
	{
		rosettaWrapper.rosetta.setLocale ("default");

		execute (this, "initialize");

		execute (coverFader, "Start");
		execute (coverFader, "setOpacity", 1.0f);
		execute (this, "cancelIAP");

		execute (justLogoutMenu, "SetActive", false);

		execute (rootMenu, "SetActive", false);
		execute (loginMenu, "SetActive", false);
		execute (createAccountMenu, "SetActive", false);
		//execute (instructionsButton, "SetActive", false);
		execute (this, "setBackButtonEnabled", false, 0);
		execute (this, "InteractableButton", false);
		execute (this, "ResetCreditsText");

		//// load stored login and check if OK
		////execute (controllerHub.saveController, "loadLoginData");

		////execute (this, "checkAppUser");

		////waitForCondition(true, "==", controllerHub.networkController, "httpRequestIsDone");
		////execute (this, "updateLoginText");
		execute (controllerHub.uiController, "fadeIn");
		//////programIf ("userOK", "", "true", "==", controllerHub.networkController, "wwwResult");
		////execute (this, "postCheckAppUser");

		execute (this, "checkQuickSaveInfoAndChooseNewGameMenu");
		execute (coverFader, "fadeToTransparent");


//		createSubprogram ("fromRootToLoginMenu");
//
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (loginNotValid, "reset");
//		execute (serverUnreachableStim, "reset");
//		execute (rootMenu, "SetActive", false);
//		execute (loginMenu, "SetActive", true);
//		execute (this, "setBackButtonEnabled", true, 1);
//		execute (coverFader, "fadeToTransparent");


//		createSubprogram ("fromNewGameToRootMenu");
//		debug ("...");
//		execute (this, "InteractableButton", false);
//		execute (this, "ResetCreditsText");
//		execute (this, "clearPassword");
//		execute (this, "loggedOutSave");
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (rootMenu, "SetActive", true);
//		execute (instructionsButton, "SetActive", true);
//		execute (newGameMenu, "SetActive", false);
//		execute (newGameMenuAlt, "SetActive", false);
//		execute (this, "setBackButtonEnabled", false, 2);
//		execute (coverFader, "fadeToTransparent");



//		createSubprogram ("fromRootToCreateAccountMenu");
//
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (newUserNotValid, "reset");
//		execute (newUserPassNoMatch, "reset");
//		execute (rootMenu, "SetActive", false);
//		execute (createAccountMenu, "SetActive", true);
//		execute (this, "setBackButtonEnabled", true, 3);
//		execute (coverFader, "fadeToTransparent");
//
//
//
//		createSubprogram ("fromCreateAccountMenuToRoot");
//
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (rootMenu, "SetActive", true);
//		execute (instructionsButton, "SetActive", true);
//		execute (createAccountMenu, "SetActive", false);
//		execute (this, "setBackButtonEnabled", false, 4);
//		execute (coverFader, "fadeToTransparent");
//
//
//
//
//		createSubprogram ("fromLoginMenuToRoot");
//		debug ("...");
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (this, "InteractableButton", false);
//		execute (this, "ResetCreditsText");
//		execute (rootMenu, "SetActive", true);
//		execute (instructionsButton, "SetActive", true);
//		execute (loginMenu, "SetActive", false);
//		execute (justLogoutMenu, "SetActive", false);
//		execute (this, "setBackButtonEnabled", false, 5);
//		execute (coverFader, "fadeToTransparent");
//		execute (this, "clearPassword");
//		execute (this, "loggedOutSave");



//		createSubprogram ("createAccount");
//
//		//execute (instructionsButton, "SetActive", false);
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (createAccountMenu, "SetActive", false);
//
//
//
//		createSubprogram ("recoverPassword");
//
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (this, "requestNewPassword");
//		execute (loginMenu, "SetActive", false);
//		//execute (instructionsButton, "SetActive", false);
//		execute (passwordRecoveryMenu, "SetActive", true);
//		execute (this, "setBackButtonEnabled", false, 6);
//		execute (coverFader, "fadeToTransparent");
//		delay (8.0f, this, "checkTouch");
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (passwordRecoveryMenu, "SetActive", false);
//		execute (rootMenu, "SetActive", true);
//		execute (this, "setBackButtonEnabled", false, 7);
//		execute (coverFader, "fadeToTransparent");



//		createSubprogram ("attemptLogin");
//
//		execute (this, "readLoginMenuPassword");
//		execute (this, "readLoginMenuUserMail");
//		execute(this, "loginDataToGameController");
//		execute (this, "checkAppUser");
//		waitForCondition(true, "==", controllerHub.networkController, "httpRequestIsDone");
//		//programIf ("userOK", "", "true", "==", controllerHub.networkController, "wwwResult");
//		execute(this, "postCheckAppUser");
//
//		programIf ("", "LoginFailed", "", "==", controllerHub.networkController, "wwwResult");
//		// server unreachable
//		execute(serverUnreachableStim, "stimulate");
//
//		// user not ok
//		createSubprogram("LoginFailed");
//		execute(loginNotValid, "stimulate");
//
//
//		createSubprogram ("preUserOK");
//		waitForTask(coverFader, "fadeToOpaque");
//		programGoTo ("userOK");
//
//		createSubprogram ("userOK");
//		// user ok
//		execute (this, "setBackButtonEnabled", true, 8);
//		execute (controllerHub.saveController, "saveLoginData");
//
//		waitForTask(coverFader, "fadeToOpaque");
//		execute (rootMenu, "SetActive", false);
//		execute (loginMenu, "SetActive", false);
//		execute (this, "checkQuickSaveInfoAndChooseNewGameMenu");
//		//execute (newGameMenu, "SetActive", true);
//		execute (coverFader, "fadeToTransparent");
//		// implicit notifyFinish()



		createSubprogram ("createNewGame");

		execute (this, "copyLoginToNetworkAgent");
		execute (this, "InteractableButton", false);
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (controllerHub.masterController, "setNextActivity", "createNewGame");
		programNotifyFinish ();



		createSubprogram ("joinGame");

		execute (this, "copyLoginToNetworkAgent");
		execute (this, "InteractableButton", false);
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		execute (controllerHub.masterController, "setNextActivity", "joinNewGame");
		programNotifyFinish ();


//		createSubprogram ("fromCreateAccountMenuToCheckMail1Menu");
//
//		waitForTask(coverFader, "fadeToOpaque");
//		execute (this, "updateCreditsHUD");
//		execute (passwordRecoveryMenu, "SetActive", true);
//		execute (this, "setBackButtonEnabled", false, 9);
//		execute (createAccountMenu, "SetActive", false);
//		execute (coverFader, "fadeToTransparent");
//		delay (1.0f);
//		delay(4.0f, this, "checkTouch");
//		waitForTask (coverFader, "fadeToOpaque");
//		execute (passwordRecoveryMenu, "SetActive", false);
//		execute (rootMenu, "SetActive", true);
//		execute (instructionsButton, "SetActive", true);
//		execute (coverFader, "fadeToTransparent");


		//run ();

		createSubprogram ("Gallery");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		waitForProgram (controllerHub.galleryController);
		execute (controllerHub.uiController, "fadeIn");


		createSubprogram ("ContinueGame");
		execute (this, "copyLoginToNetworkAgent");
		execute (this, "InteractableButton", false);
		execute (controllerHub.masterController, "setNextActivity", "ContinueGame");
		waitForTask (controllerHub.uiController, "fadeOutTask", this);
		programNotifyFinish ();



//		createSubprogram ("AwaitingForCorrectMagic");
//		debug ("...");
//		debug ("...");
//		execute (this, "updateCreditsHUD");
//		execute (new FGPMethodCall (controllerHub.uiController, "fadeIn"));
//		execute (new FGPMethodCall (coverFader, "fadeToTransparent"));
//		execute (new FGPMethodCall (createAccountMenu, "SetActive", false));
//		execute (new FGPMethodCall (rootMenu, "SetActive", false));
//		execute (new FGPMethodCall (loginMenu, "SetActive", false));
//		execute (new FGPMethodCall (this, "setBackButtonEnabled", false, 12));
//		//execute (new FGPMethodCall (noMoarMagicMenu, "SetActive", true));
//		//execute (new FGPMethodCall (m, "SetActive", false));
//		delay (0.25f);
//		//execute (new FGPMethodCall (titleScaler, "scaleIn"));
//		execute (new FGPMethodCall (noMoarMagicPanel, "scaleIn"));


	}

	public void checkTouch() {
		if (Input.GetMouseButtonDown (0))
			cancelDelay ();
	}
	
	public void requestNewPassword() {
		
		string user = LoginMenu_usermailText.text;
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("email", user);
		WWW www = new WWW (controllerHub.networkController.bootstrapData.loginServer + ":" + controllerHub.networkController.bootstrapData.loginServerPort + FGUtils.RecoveryScript, wwwForm);

	}

	// UI callback methods
	public void createAccountButton() {
		goTo ("fromRootToCreateAccountMenu");
	}
		

	public void createNewAccountButton() {
		if (!FGUtils.isValideMail (NewAccount_usermailText.text)) {
			newUserNotValid.stimulate ();
		}
		else if ((!NewAccount_pass1Text.text.Equals(NewAccount_pass2Text.text)) && 
			(!NewAccount_pass1Text.text.Equals("")) &&
			(!NewAccount_pass2Text.text.Equals(""))
		) {
			
			newUserPassNoMatch.stimulate();
		}
		else {
			WWWForm wwwForm = new WWWForm();
			wwwForm.AddField("email", NewAccount_usermailText.text);
			wwwForm.AddField("passwd", NewAccount_pass1Text.text);
			wwwForm.AddField ("magic", NewAccount_magicText.text);
			WWW www = new WWW (controllerHub.networkController.bootstrapData.loginServer + ":" + controllerHub.networkController.bootstrapData.loginServerPort  + "/newUser", wwwForm);
			goTo ("fromCreateAccountMenuToCheckMail1Menu");
		}
	}

	public void ClickOnPDF()
	{
		Application.OpenURL (PDFLink);
	}

	public void ClickOnVideo()
	{
		Application.OpenURL (VideoLink);
	}

	public void clickOnInfo()
	{
		instructionsPanel.Start ();
		instructionsPanel.gameObject.SetActive (true);
		instructionsPanel.scaleIn ();
	}

	public void clickOnCloseInfo()
	{
		instructionsPanel.scaleOut ();
	}

	public void loginButton() {
		goTo ("fromRootToLoginMenu");
	}

	public void continueButton() {
		goTo ("ContinueGame");
	}

	public void createNewGameButton() {

		if (!freePlay) {
			if (playcodeInput.text != CorrectPlayCode)
				return;

			controllerHub.gameController.quickSaveInfo.playcode = playcodeInput.text;
			controllerHub.saveController.saveQuickSaveInfo ();
		}


		//if (accountCredits > 0) {
			goTo ("createNewGame");
		//} else {
		//	noMoarCreditsPanel.scaleIn ();
		//}
	}

	public void createNewGameConfirmButton() {

		//if (playcodeInput.text != CorrectPlayCode)
		//	return;

		controllerHub.gameController.quickSaveInfo.playcode = playcodeInput.text;
		controllerHub.saveController.saveQuickSaveInfo ();

		//goTo ("createNewGame");
		//if (accountCredits > 0) {
			whichProgram = "createNewGame";
			confirmDeleteSessionMenu.scaleIn ();
		//} else {
		//	noMoarCreditsPanel.scaleIn ();
		//}
	}

	public void joinGameButton() {
		//if (accountCredits > 0) {
			goTo ("joinGame");
		//} else {
		//	noMoarCreditsPanel.scaleIn ();
		//}
	}

	public void joinGameConfirmButton() {
		//goTo ("joinGame");
		//if (accountCredits > 0) {
			whichProgram = "joinGame";
			confirmDeleteSessionMenu.scaleIn ();
		//} else {
		//	noMoarCreditsPanel.scaleIn ();
		//}
	}

	public void submitCreateAccountButton() {
		goTo ("createAccount");
	}

	public void loginSubmitButton() {
		goTo ("attemptLogin");
	}

	public void recoverPasswordButton() {
		goTo ("recoverPassword");
	}

	public void deletePreviousSessionYesButton() {
		confirmDeleteSessionMenu.scaleOut ();
		controllerHub.saveController.resetQuickSaveInfo();
		goTo (whichProgram);

	}

	public void deletePreviousSessionNoButton() {
		confirmDeleteSessionMenu.scaleOut ();
	}

	public void backButton() {
		string currentState = subprogramName ();
		if (currentState.Equals ("fromRootToCreateAccountMenu")) {
			goTo ("fromCreateAccountMenuToRoot");
		} else if (currentState.Equals ("fromRootToLoginMenu") || currentState.Equals("LoginFailed") || currentState.Equals("attemptLogin")) {
			goTo ("fromLoginMenuToRoot");
		} else if (currentState.Equals ("userOK")) {
			goTo ("fromNewGameToRootMenu");
		}
	}

	public void InteractableButton(bool _interac)
	{
		buyButton.GetComponent<Button> ().interactable = _interac;
	}

	public void ResetCreditsText()
	{
		Debug.Log ("ResetCreditsText");
		creditsHUD.text = "Créditos: ";
	}

	public void updateCreditsHUD() {

		if (accountCredits >= 0) {
			creditsHUD.text = "Créditos: " + accountCredits;
			InteractableButton (true);
		} else if (accountCredits == -1) {
			creditsHUD.text = "Créditos: ∞";
			InteractableButton (false);
		} else if (accountCredits == -2) {
			creditsHUD.text = "Créditos: ?";
			InteractableButton (false);
		} else {
			creditsHUD.text = "Créditos: ";
			InteractableButton(false);
		}

	}

	public void tacOKButton() {
		if (taccanvas.activeSelf == true)
			taccanvas.SetActive (false);
		else
			taccanvas.SetActive (true);
	}

	public void cancelIAP() {
		IAPCanvas.SetActive (false);
	}

	public void clickOnLogout() {
		goTo ("fromLoginMenuToRoot");
		noMoarMagicPanel.scaleOut ();
	}


	public void clickOnCancel() {
		//noMoarPanel.setEasyType(EaseType.linear);
		noMoarCreditsPanel.scaleOut();
		noMoarMagicPanel.scaleOut ();
	}

	public void clickOnBuy() {
		controllerHub.masterController.toggleServicePanel ();
		noMoarMagicPanel.scaleOut ();
		IAPCanvas.SetActive (true);
	}

	public void clickOnEnterNewMagicButton() {
		string user = controllerHub.gameController.gameState.localEMail;
		string pass = controllerHub.gameController.gameState.localPasswd;
		string newMagic = newMagicInput.text;
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("email", user);
		wwwForm.AddField ("passwd", pass);
		wwwForm.AddField ("magic", newMagic);
		WWW www = new WWW (controllerHub.networkController.bootstrapData.loginServer + ":" + controllerHub.networkController.bootstrapData.loginServerPort + "/updateMagic", wwwForm);
		while (!www.isDone) {
		} // oh, no, you don't!!!
		//noMoarMagicPanel.setEasyType (EaseType.linear);
		noMoarMagicPanel.scaleOut ();
		newMagicInput.text = "";
		controllerHub.masterController.nuke ();
	}


}
