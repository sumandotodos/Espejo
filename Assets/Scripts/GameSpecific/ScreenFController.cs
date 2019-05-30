using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WordGameResult { wordOK, timeup, none };

public class ScreenFController : FGProgram {

	public Animator animToVertical;

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;
	public WellTimer wtimer;

//	public string stringBankPrefix;
//	public int stringBankLength;
	public FGTable scienceCloze;
	public FGTable mindCloze;
	FGTable table;

	public bool isScienceCloze;

	public Text textete;
	public UITextFader textFader;

	int phraseIndex;

	public UIDrawHide screenBPanel;

	public UIScaleFader helpButtonScaler;
	public UIScaleFader helpPanelScaler;

	public ScrambledWordGame.SWGGameController wordGameController;

	public RosettaWrapper rosettaWrapper;

	string phrase;
	string textWithHoles;
	List<string> clozeText;

	int word = 0;

	public WordGameResult wordGameResult = WordGameResult.none;

	List<string> extractClozeText(string text, out string textWithHoles) {

		textWithHoles = "";
		List<string> result = new List<string> ();
		char[] charArray = text.ToCharArray ();
		bool clozing = false;
		int start = 0, end = 0;
		for (int i = 0; i < charArray.Length; ++i) {
			if (charArray [i] == '_') {
				if (!clozing) {
					clozing = true;
					start = i + 1;
				} else {
					clozing = false;
					end = i;
					result.Add (text.Substring (start, end - start));
				}
			} else {
				if (!clozing)
					textWithHoles += charArray [i].ToString ();
				else {
					textWithHoles += "_ ";
				}
			}
		}
		return result;
	}

	public void retrievePhraseAtRandom() 
	{
		string withHoles = "";
		int r = Random.Range (0, table.nRows());
		phrase = (string)table.getElement (0, r);//rosetta.retrieveString (stringBankPrefix, r);
		clozeText = extractClozeText (phrase, out withHoles);
		textete.text = withHoles;
		textWithHoles = withHoles;
		phraseIndex = r;
		//controllerHub.gameController.addPhrase (r, isScienceCloze);
		word = 0;
	}

	public static string underslashReplace(string withHoles, string word) {

		int firstUnderslash = withHoles.IndexOf ('_');
		string newString = withHoles.Substring (0, firstUnderslash);
		newString += word;
		int finishIndex = firstUnderslash + word.Length * 2;
		newString += withHoles.Substring (finishIndex, withHoles.Length - finishIndex); 
		return newString;
	}

	public void startWordGame()
	{		
		string tx = clozeText [word];

		//wtimer.frameTime = textWithHoles.Length / 32.0f;
		wtimer.frameTime = (textWithHoles.Length / 100) + (clozeText [word].Length * 0.25f);
		wtimer.Start ();
		wtimer.reset ();
		wtimer.go ();
		wordGameController.go (tx);

		word++;
	}

	public void updateTextWithHoles() {
		if (wordGameResult != WordGameResult.timeup) {
			textete.text = underslashReplace (textete.text, clozeText [word - 1]);
		}
	}

	public bool isThereANextWord() {
		return ((word < clozeText.Count) && (wordGameResult != WordGameResult.timeup));
	}

	public void setScreenCPhrase() {
		controllerHub.screenCController.setPhraseIndex(isScienceCloze, phraseIndex);
		controllerHub.screenCController.firstPhraseIndex = phraseIndex;
	}

	public void addPhraseToPlayer() {
		controllerHub.gameController.addPhrase (phraseIndex, isScienceCloze);
//		if (isScienceCloze) {
//			controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].sciencePhrasesObtained.Add (phraseIndex);
//		} else {
//			controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mindPhrasesObtained.Add (phraseIndex);
//		}
	}

	public void resetWordGameResult() {
		wordGameController.stop ();
		wordGameResult = WordGameResult.none;
		word = 0;
	}

	public void wordGamePostMortem() {
		if (wordGameResult != WordGameResult.timeup) {
			controllerHub.networkController.sendCommand (controllerHub.gameController.gameState.masterLogin, 
				FGNetworkManager.makeClientCommand ("winwordgame", controllerHub.gameController.gameState.localNPlayer));
		}
		animToVertical.SetTrigger ("Turn");
	}

	public void setUpTable() {
		if (isScienceCloze) {
			table = scienceCloze;
		} else {
			table = mindCloze;
		}
	}

	public void stopTimer() {
		wtimer.stop ();
	}

	public void helpButtonPress() {
		helpButtonScaler.scaleOut ();
		helpPanelScaler.scaleIn ();
	}

	public void closeHelp() {
		helpPanelScaler.scaleOut ();
		helpButtonScaler.scaleIn ();
	}

	void Start () 
	{
		execute (this, "resetWordGameResult");
		execute (this, "setUpTable");
		execute (this, "retrievePhraseAtRandom");


		//execute (this, "resetWordGameResult");
		execute (controllerHub.screenKController, "setGetKey", false);

		//execute (this, "retrievePhraseAtRandom");
		//delay (0.5f);
		execute (textFader, "fadeToOpaque");
		//startWhile (this, "isThereANextWord");

		execute (this, "startWordGame");
		waitForTask (screenBPanel, "hide");
		execute (canvasHub.screenBCanvas, "SetActive", false);
		waitForProgram (wordGameController);
		execute (this, "stopTimer");
		execute (this, "updateTextWithHoles");
		//endWhile ();
		execute (this, "wordGamePostMortem");
		delay (3.0f);
		execute (this, "setScreenCPhrase");
		execute (this, "addPhraseToPlayer");
		waitForProgram (controllerHub.screenCController);
		//debug ("Program_F_finish");
		programNotifyFinish ();

	}
	
	// Update is called once per frame
	void Update () {
		update ();
	}
}
