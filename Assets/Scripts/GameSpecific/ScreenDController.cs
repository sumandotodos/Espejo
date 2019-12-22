using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDController : FGProgram {

	public UITip mindTurnTip;
	public UITip mindNonTurnTip;
	public UITip scienceTurnTip;
	public UITip scienceNonTurnTip;

	public AudioClip clickAudio;

	public const int CommonAnswer = 0;
	public const int IndividualAnswer = 1;

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public GameObject screenDAnswerPanel;

	public UIDrawHide panelDDrawHide;

	public UIAnimatedImage mirrorDamageImage;
	public Text mirrorDamageText;

	FGTable table;
	public FGTable scienceTable;
	public FGTable mindTable;

	public Text questionText;
	public Text answerAText;
	public Text answerBText;
	public Text answerCText;

	public RawImage[] correctIndicator;

	public GameObject mirrorGlow;

	public UIFader[] presuntoFaders;

	public UIImageToggle[] toggles;

	public UITextFader comunLabelFader;
	public UITextFader individualLabelFader;

	public UITargetAnimation coin;

	public string screenDHost;



	public bool clickedOnOption = false;
	int timesClicked = 0;

	int correctOne = -1;

	public bool initialized = false;

	public bool scienceQuestion = false;

	public int commonAnswer = -1; // this will be set to other than -1 if there is consensus
	public int[] commonAnswerFromOthers;
	public List<int> individualAnswerFromOthers;

	bool lockComun, lockIndi;

	public int ownCommonResponse;
	public int ownIndividualResponse;
	public int presuntoCorrectOne = -1;

	// key stuff; unpublic them
	public int color;
	public int index;
	public int type;
	public int whatRow;


	public bool allPlayersSameAnswer() {
		
		if (commonAnswer != -1) {
			setButtonDisableOthersEnabled (true); // prevent a user from changing the common answer due to network lag
			toggles [commonAnswer].toggle (); 
			return true;
		}
		return false;
	}

	public void setScienceQuestion(bool sc) {
		scienceQuestion = sc;
	}

	public bool hasClickedOnOption() {
		return clickedOnOption;
	}
	public void resetClickedOnOption() {
		clickedOnOption = false;
		timesClicked++;
	}

	bool processingCommonAnswer = false;

	public void setProcessingCommonAnswer(bool b) {
		processingCommonAnswer = b;
	}

	public void clickOnOption(int op) {

		if (clickedOnOption == true)
			return;

		clickedOnOption = true;

		if (controllerHub.gameController.isMyTurn ()) {



			presuntoFaders [op].setOpacity (1.0f);
			if (isScienceQuestion ()) {
				presuntoCorrectOne = op;
				controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("presuntoanswer", op));
				if (op == correctOne)
					goTo ("ScienceTellTruth");
				else
					goTo ("ScienceTellLies");
			} else {
				presuntoCorrectOne = op;
				controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("presuntoanswer", op));
				goTo ("MindTurnFinish");
			}

		} else { // not my turn player

			int thisPlayer = controllerHub.gameController.gameState.localNPlayer;

			if (processingCommonAnswer) {
				
					controllerHub.networkController.sendCommand (screenDHost, 
						FGNetworkManager.makeClientCommand ("answer", thisPlayer, CommonAnswer, op));
					clickedOnOption = false; // moar clicks are permitted

			} else {
				if (!lockIndi) {
					ownIndividualResponse = op;
					controllerHub.networkController.broadcast (
						FGNetworkManager.makeClientCommand ("answer", thisPlayer, IndividualAnswer, op));
					lockIndi = true;
				}
			}


		}

		controllerHub.audioController.playSound (clickAudio);

	}

	public void resetPressedButtons() {
		for (int i = 0; i < toggles.Length; ++i) {
			toggles [i].reset ();
		}
	}
	public void unlockButtons() {
		for (int i = 0; i < toggles.Length; ++i) {
			toggles [i].setEnabled (true);
		}
		clickedOnOption = false;
	}
	public void lockButtons() {
		for (int i = 0; i < toggles.Length; ++i) {
			toggles [i].setEnabled (false);
		}
		clickedOnOption = true;
	}

	public void deinitializeScreen() {
		
		initialized = false;
	}

	public bool screenIsInitialized() {
		return initialized;
	}

	bool truthMirror = false;

	public bool haveTruthMirror() {
		return truthMirror;
	}

	public void lightMirrorGlow() {
		truthMirror = true;
		mirrorGlow.SetActive (true);
		if (correctOne >= 0) {
			correctIndicator [correctOne].enabled = true;
		}
		if ((!isScienceQuestion ()) && (presuntoCorrectOne != -1)) {
			presuntoFaders [presuntoCorrectOne].fadeToOpaque ();
		}
	}

	public void initializeScreen() {

		advances = 0;

		presuntoCorrectOne = -1;

		truthMirror = false;

		int otherPlayer = -1;

		setButtonDisableOthersEnabled (true);

		commonAnswer = -1;

		clickedOnOption = false;
		timesClicked = 0;
		for (int i = 0; i < toggles.Length; ++i) {
			toggles[i].Start ();
			toggles [i].reset ();
			toggles [i].setEnabled (true);
		}
		for (int i = 0; i < correctIndicator.Length; ++i) {
			correctIndicator [i].enabled = false;
		}
		for (int i = 0; i < presuntoFaders.Length; ++i) {
			presuntoFaders [i].Start ();
			presuntoFaders [i].setOpacity (0.0f);
		}

		panelDDrawHide.Start ();
		panelDDrawHide.reset ();
		if (controllerHub.gameController.isMyTurn ()) {
			screenDAnswerPanel.SetActive (true);
			//scienceQuestion = (Random.Range (0, 2) == 0);
			if (scienceQuestion) {
				table = scienceTable;
				coin.setFrame (8);
				scienceTurnTip.show ();
			} else {
				table = mindTable;
				coin.setFrame (0);
				mindTurnTip.show ();
			}
			turnPlayerInitialization ();
		} else {
			screenDAnswerPanel.SetActive (false);
//			answerAText.text = "";
//			answerBText.text = "";
//			answerCText.text = "";
		}
		individualLabelFader.Start ();
		individualLabelFader.setOpacity (0.0f);
		comunLabelFader.Start ();
		comunLabelFader.setOpacity (0.0f);

		lockComun = lockIndi = false;

		mirrorGlow.SetActive (false);

		lockButtons ();



	}

	// called from network
	public void presuntoAnswer(int a) {
		if (isScienceQuestion ()) {
			presuntoFaders [a].fadeToOpaque ();
		} 
		else {
			if (haveTruthMirror ()) {
				presuntoFaders [a].fadeToOpaque ();
			}
		}
		presuntoCorrectOne = a;
	}

	// called from network
	public void notMyTurnInitialization(int row, bool sc) {

		whatRow = row;

		//StringBank linksSB;

		scienceQuestion = sc;
		if (scienceQuestion) {
			table = scienceTable;
			coin.setFrame (8);
			//int.TryParse (table.getElement (4, row), out correctOne);
			correctOne = (int)table.getElement(4, row);
			//linksSB = table.getColumn (5);
			type = GameController.Science;
		} else {
			table = mindTable;
			coin.setFrame (0);
			correctOne = -1;
			//linksSB = table.getColumn (4);
			type = GameController.Mind;
		}
		string A = (string)table.getElement (1, row);
		string B = (string)table.getElement (2, row);
		string C = (string)table.getElement (3, row);

        if (scienceQuestion)
        {
            scienceNonTurnTip.show();
        }
        else
            mindNonTurnTip.show();

        index = row;
		if (scienceQuestion) {
			index = (int)table.getElement (5, row)-1;
		}
		color = FGUtils.pseudoRandom (index) % GalleryController.NColors;

		int damage = controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mirrorDamage;
		mirrorDamageImage.Start ();
		mirrorDamageImage.setFrame (damage);
		mirrorDamageText.text = "" + damage;

		answerAText.text = A;
		answerBText.text = B;
		answerCText.text = C;

		initialized = true;

	}

	public void turnPlayerInitialization() {

		//StringBank linkSB;

		individualAnswerFromOthers = new List<int> ();
		
		int row = table.getNextRowIndex ();
		whatRow = row;
		string question = (string)table.getElement (0, row);
		string A = (string)table.getElement (1, row);
		string B = (string)table.getElement (2, row);
		string C = (string)table.getElement (3, row);
		if (isScienceQuestion ()) {
			//int.TryParse (table.getElement (4, row), out correctOne);
			correctOne = (int)table.getElement (4, row);
			//linkSB = table.getColumn (5);
			type = GameController.Science;
		} else {
			correctOne = -1;
			//linkSB = table.getColumn (4);
			type = GameController.Mind;
		}

		index = row;
		if (scienceQuestion) {
			index = (int)table.getElement (5, row)-1;
		}
		color = FGUtils.pseudoRandom (index) % GalleryController.NColors;//0; //linkSB.color [row];

		int damage = controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mirrorDamage;
		mirrorDamageImage.Start ();
		mirrorDamageImage.setFrame (damage);
		mirrorDamageText.text = "" + damage;

		questionText.text = question;
		answerAText.text = A;
		answerBText.text = B;
		answerCText.text = C;

		unlockButtons ();

		if (correctOne != -1) {
			correctIndicator [correctOne].enabled = true;
		}

		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("whitequestion", row, scienceQuestion));


//		int otherPlayer;
//		// decide one of the rest of players
//		otherPlayer = Random.Range (0, GameController.MaxPlayers);
//		while ((otherPlayer == controllerHub.gameController.gameState.localNPlayer) ||
//		      (!controllerHub.gameController.gameState.playerList [otherPlayer].isPresent)) {
//			otherPlayer = Random.Range (0, GameController.MaxPlayers);
//		}
//		controllerHub.networkController.sendCommand (controllerHub.gameController.gameState.playerList [otherPlayer].login, 
//			FGNetworkManager.makeClientCommand ("lightmirror"));
		// NOBODY HAS THE MIRROR OF TRUTH
		

		commonAnswerFromOthers = new int[GameController.MaxPlayers];
		for(int i = 0; i < GameController.MaxPlayers; ++i) {
			commonAnswerFromOthers[i] = -1;
		}


		initialized = true;

	}

	public bool hasReceivedPresuntoCorrectOne() {
		return (presuntoCorrectOne != -1);
	}

	public bool isScienceQuestion() {

		return scienceQuestion; //

	}

	public void damageMirror() {

		int damage = (controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mirrorDamage);
		if (damage < 4)
			++damage;
		controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mirrorDamage = damage;
		mirrorDamageImage.Start ();
		mirrorDamageImage.setFrame (damage);
		mirrorDamageText.text = "" + damage;

	}

	public void setButtonDisableOthersEnabled(bool en) {
		for (int i = 0; i < toggles.Length; ++i) {
			toggles [i].mustDisableOthers = en;
		}
	}

	public void prepareScreenG() {
		controllerHub.screenGController.setKeyType (type);
		controllerHub.screenGController.setKeyColor (color);
		controllerHub.screenGController.setKeyIndex (index);
	}

	public void prepareScreenKPrime(bool getsKey) {
		controllerHub.screenKPrimeController.setGetKey (getsKey);
		controllerHub.screenKPrimeController.setAdvances (advances);
	}

	public int advances;
	public bool enoughIndividualAnswers() {
		if (individualAnswerFromOthers.Count == 0)
			return false;
		return (individualAnswerFromOthers.Count >= (controllerHub.gameController.gameState.nPlayers - 2));
	}

	public bool hasCommonAnswer() {
		return (commonAnswer != -1);
	}

	// we have to have common answer, and individualAnswerFromOthers.Count == nplayers - 2
	//   hasCommonAnswer() -> bool & enoughIndividualAnswers() -> bool
	public void calculateAdvance_mindTurn() {

		advances = 0;
		if (commonAnswer == presuntoCorrectOne) {
			advances += (controllerHub.gameController.gameState.nPlayers - 1);
		}
		for (int i = 0; i < individualAnswerFromOthers.Count; ++i) {
			if (individualAnswerFromOthers [i] == presuntoCorrectOne) {
				++advances;
			}
		}

		if (advances > 3)
			advances = 3;

	}


	public void calculateAdvance_mindNoTurn() {

		advances = 0;
		if (presuntoCorrectOne == ownCommonResponse)
			++advances;
		if (presuntoCorrectOne == ownIndividualResponse)
			++advances;

	}

	// can only be called when we have received presunto answer from turn player
	// and common answer from network
	// hasReceivedPresuntoCorrectOne() -> bool & hasCommonAnswer() -> bool
	public void calculateAdvance_mindNoTurnMirror() {

		advances = 0;
		if (commonAnswer != presuntoCorrectOne) {
			advances = controllerHub.gameController.gameState.nPlayers - 2;
		}
			

	}

	// we have to have common answer, and individualAnswerFromOthers.Count == nplayers - 2
	//   hasCommonAnswer() -> bool & enoughIndividualAnswers() -> bool
	public void calculateAdvance_scienceTurn() {

		advances = 0;
		if (presuntoCorrectOne != correctOne) { // sólo puntua si miente
			if (commonAnswer == presuntoCorrectOne) {
				advances += (controllerHub.gameController.gameState.nPlayers - 1);
			}
			for (int i = 0; i < individualAnswerFromOthers.Count; ++i) {
				if (individualAnswerFromOthers [i] == presuntoCorrectOne) {
					++advances;
				}
			}
		}

		if (advances > 3)
			advances = 3;

	}


	public void calculateAdvance_scienceNoTurn() {

		advances = 0;
		if (correctOne == ownCommonResponse)
			++advances;
		if (correctOne == ownIndividualResponse)
			++advances;

	}

	// can only be called when we have received presunto answer from turn player
	// and common answer from network
	// hasReceivedPresuntoCorrectOne() -> bool & hasCommonAnswer() -> bool
	public void calculateAdvance_scienceNoTurnMirror() {

		if ((commonAnswer != correctOne) && (commonAnswer != presuntoCorrectOne)) {
			advances = controllerHub.gameController.gameState.nPlayers - 2;
		}
	}

	public void calculateAdvances() {

		if (isScienceQuestion ()) {
			if (controllerHub.gameController.isMyTurn ()) {
				calculateAdvance_scienceTurn ();
			} else {
				if (haveTruthMirror ()) {
					calculateAdvance_scienceNoTurnMirror ();
				} else
					calculateAdvance_scienceNoTurn ();
			}
		} else {
			if (controllerHub.gameController.isMyTurn ()) {
				calculateAdvance_mindTurn ();
			} else {
				if (haveTruthMirror ()) {
					calculateAdvance_mindNoTurnMirror ();
				} else
					calculateAdvance_mindNoTurn ();
			}
		}

	}

	// Use this for initialization
	void Start () {

		execute (this, "initializeScreen");
		waitForCondition (this, "screenIsInitialized");
		execute (canvasHub.screenDCanvas, "SetActive", true);

		//
		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		//

		waitForTask (panelDDrawHide, "show");
		execute (canvasHub.screenBCanvas, "SetActive", false);
		execute (canvasHub.screenAPrimeCanvas, "SetActive", false);


		programIf ("Science", "Mind", true, "==", this, "isScienceQuestion");


		createSubprogram ("Science");
		programIf (FGProgram.CONTINUE, "ScienceNoTurnPlayer", true, "==", controllerHub.gameController, "isMyTurn");
		execute (this, "unlockButtons");
		// myTurn waits for UI input...

		createSubprogram ("ScienceNoTurnPlayer");
		delay (0.5f);
		execute (this, "setButtonDisableOthersEnabled", false);
		execute (this, "setProcessingCommonAnswer", true);
		execute (comunLabelFader, "fadeToOpaque");
		execute (this, "unlockButtons");
		waitForCondition (this, "allPlayersSameAnswer");
		execute (comunLabelFader, "fadeToTransparent");
		execute (this, "setButtonDisableOthersEnabled", true);
		execute (this, "setProcessingCommonAnswer", false);
		programIf ("ScienceHasMirror", "ScienceDoesntHaveMirror", true, "==", this, "haveTruthMirror");

		createSubprogram ("ScienceDoesntHaveMirror");
		delay (1.0f);
		execute (this, "resetPressedButtons");
		execute (this, "resetClickedOnOption");
		execute (this, "unlockButtons");
		execute (individualLabelFader, "fadeToOpaque");
		waitForCondition (this, "hasClickedOnOption");
		execute(individualLabelFader, "fadeToTransparent");
		programGoTo ("ScienceHasMirror");

		createSubprogram ("ScienceHasMirror");
		//execute (this, "lockButtons");
		delay (1.0f);
		execute (this, "resetPressedButtons");
		execute (this, "lockButtons");
		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		waitForCondition (this, "hasReceivedPresuntoCorrectOne");
		waitForCondition(this, "hasCommonAnswer");
		execute (this, "calculateAdvances");
		execute(this, "prepareScreenG");
		execute (this, "prepareScreenKPrime", true);
		//waitForProgram(controllerHub.screenGController);
		waitForProgram(controllerHub.screenKPrimeController);
		execute (this, "deinitializeScreen");
		programNotifyFinish ();



		createSubprogram ("ScienceTellTruth");
		execute (this, "lockButtons");
		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		waitForCondition(this, "hasCommonAnswer");
		waitForCondition (this, "enoughIndividualAnswers");
		execute (this, "calculateAdvances");
		execute(this, "prepareScreenG");
		execute (this, "prepareScreenKPrime", true);
		//waitForProgram(controllerHub.screenGController);
		waitForProgram(controllerHub.screenKPrimeController);
		execute (this, "deinitializeScreen");
		programNotifyFinish ();



		createSubprogram ("ScienceTellLies");
		delay (0.5f);
		execute (this, "damageMirror");
		delay (1.0f);
		programGoTo("ScienceTellTruth");




		createSubprogram ("Mind");
		programIf (FGProgram.CONTINUE, "MindNoTurnPlayer", true, "==", controllerHub.gameController, "isMyTurn");
		execute (this, "unlockButtons");

		createSubprogram ("MindTurnFinish");
		//execute (this, "lockButtons");
		delay (1.0f);
		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		waitForCondition(this, "hasCommonAnswer");
		waitForCondition (this, "enoughIndividualAnswers");
		execute (this, "calculateAdvances");
		execute(this, "prepareScreenG");
		execute (this, "prepareScreenKPrime", false);
		waitForProgram(controllerHub.screenKPrimeController);
		execute (this, "deinitializeScreen");
		programNotifyFinish ();



		createSubprogram ("MindNoTurnPlayer");
		delay (0.5f);
		execute (this, "setButtonDisableOthersEnabled", false);
		execute (this, "setProcessingCommonAnswer", true);
		execute (comunLabelFader, "fadeToOpaque");
		execute (this, "unlockButtons");
		waitForCondition (this, "allPlayersSameAnswer");
		execute (comunLabelFader, "fadeToTransparent");
		execute (this, "setButtonDisableOthersEnabled", true);
		execute (this, "setProcessingCommonAnswer", false);
		programIf ("MindHasMirror", "MindDoesntHaveMirror", true, "==", this, "haveTruthMirror");

		createSubprogram ("MindDoesntHaveMirror");
		delay (1.0f);
		execute (this, "resetPressedButtons");
		execute (this, "resetClickedOnOption");
		execute (this, "unlockButtons");
		execute (individualLabelFader, "fadeToOpaque");
		waitForCondition (this, "hasClickedOnOption");
		execute(individualLabelFader, "fadeToTransparent");
		programGoTo ("MindHasMirror");

		createSubprogram ("MindHasMirror");
		//execute (this, "lockButtons");
		delay (1.0f);
		execute (this, "resetPressedButtons");
		execute (this, "lockButtons");
		execute (controllerHub.uiController, "startWait");
		execute (controllerHub.networkController, "broadcast", FGNetworkManager.makeClientCommand ("sync"));
		execute (controllerHub.gameController, "addSyncPlayers");
		waitForCondition (true, "==", controllerHub.gameController, "playersAreSynced"); // sync players
		execute (controllerHub.uiController, "endWait");
		waitForCondition (this, "hasReceivedPresuntoCorrectOne");
		waitForCondition(this, "hasCommonAnswer");
		execute (this, "calculateAdvances");
		execute(this, "prepareScreenG");
		execute (this, "prepareScreenKPrime", false);
		waitForProgram(controllerHub.screenKPrimeController);
		execute (this, "deinitializeScreen");
		programNotifyFinish ();
	}

	// network callback, received by screenD guests
	public void setCommonAnswer(int a) {
		ownCommonResponse = a;
		commonAnswer = a;
	}

	// network callbacks, received by screenD host
	public void receiveResponseFromOther(int pl, int answerType, int answer) {


			
		if (answerType == CommonAnswer) {

			commonAnswerFromOthers [pl] = answer;
			int nAgreedPlayers = 0;
			for (int i = 0; i < GameController.MaxPlayers; ++i) {
				if (commonAnswerFromOthers [i] == answer)
					++nAgreedPlayers;
			}
			if (nAgreedPlayers == (controllerHub.gameController.gameState.nPlayers - 1)) {
				commonAnswer = answer;
				controllerHub.networkController.broadcast (
					FGNetworkManager.makeClientCommand ("setcommonanswer", commonAnswer));
			}

		} else {

			individualAnswerFromOthers.Add (answer);

		}



	}


}
