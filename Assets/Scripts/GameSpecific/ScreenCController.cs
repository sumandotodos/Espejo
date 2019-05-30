using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;

public class ScreenCController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public Texture velaApagadaTex;
	public RawImage velaIconRI;

	//public UIScaleFader buhoScaler;

//	public string stringBankPrefix;
//	public int stringBankLength;
	public FGTable sciencePhrases;
	public FGTable mindPhrases;
	public RosettaWrapper rosettaWrapper;

	public UIDrawHide screenCDrawHide;

	public Text phraseText;

	bool coinUsed = false;
	public bool isMindPhrase = true;

	public UITargetAnimation targetAnimation;

	int refreshStoryStart_cd = 4;
	int storyStartsIndex = 0;
	List<int> storyStarts;

	public Text storyStartText;
//	public string storyStartSBName;
//	public int storyStartSBLength;
	public FGTable storyStartTable;
	int storyIndex;

	string phrase;
	int phraseIndex;

	int otherPhraseIndex;
	public int firstPhraseIndex;

	public void setPhraseIndex(bool science, int i) {
		phraseIndex = i;
		string ph;// = rosetta.retrieveString (stringBankPrefix, phraseIndex);
		if (science) {
			ph = (string)sciencePhrases.getElement (0, phraseIndex);
			controllerHub.screenKController.setGetKey (true);
			int keyIndex = (int)sciencePhrases.getElement (1, phraseIndex);
			controllerHub.gameController.addKey (0, keyIndex, System.DateTime.Now);
			controllerHub.screenGController.setKeyColor (keyIndex % GalleryController.NColors);
		} else {
			ph = (string)mindPhrases.getElement (0, phraseIndex);
		}
		isMindPhrase = !science;
		ph = ph.Replace ("_", "");
		phraseText.text = ph;
	}

	public void setPhrase(string p) {
		phrase = p;
		phraseText.text = phrase;
	}

	public void coinFlip() {

		bool science;


		if (!isMindPhrase) {
			targetAnimation.setTargetFrame (0);
			science = false;
		} else {
			targetAnimation.setTargetFrame (8);
			science = true;
		}

		if (!coinUsed) {
			int r = 0;
			if (science) {
				r = Random.Range (0, sciencePhrases.nRows ()); //Random.Range (0, stringBankLength);
				if (controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].sciencePhrasesObtained.Count < sciencePhrases.nRows ()) {
					for (int i = 0; i < sciencePhrases.nRows (); ++i) {
						if (controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].sciencePhrasesObtained.Contains (r)) {
							r = (r + 1) % sciencePhrases.nRows ();
						} else
							break;
					}
				}
				controllerHub.screenKController.setGetKey (true);
				int keyIndex = (int)sciencePhrases.getElement (1, r);
				controllerHub.gameController.addKey (0, keyIndex, System.DateTime.Now);

			} else {
				r = Random.Range (0, mindPhrases.nRows ()); //Random.Range (0, stringBankLength);
				if (controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mindPhrasesObtained.Count < mindPhrases.nRows ()) {
					for (int i = 0; i < mindPhrases.nRows (); ++i) {
						if (controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].mindPhrasesObtained.Contains (r)) {
							r = (r + 1) % mindPhrases.nRows ();
						} else
							break;
					}
				}

			}
			controllerHub.gameController.addPhrase (r, science);
			otherPhraseIndex = r;
		}

		coinUsed = true;
		if (phraseIndex == firstPhraseIndex)
			setPhraseIndex (science, otherPhraseIndex);
		else
			setPhraseIndex (science, firstPhraseIndex);


	}

	public void initScreen() {
		storyStarts = new List<int> ();
		int nextRow = storyStartTable.getNextRowIndex ();
		storyStartText.text = (string)storyStartTable.getElement(0, nextRow);
		storyIndex = nextRow;
		storyStarts.Add (nextRow);
		coinUsed = false;
		if (isMindPhrase) {
			targetAnimation.setTargetFrame (0);
			targetAnimation.setFrame (0);
		} else {
			targetAnimation.setTargetFrame (8);
			targetAnimation.setFrame (8);
		}
		velaIconRI.gameObject.GetComponent<UIAnimatedImage> ().Start ();
		velaIconRI.gameObject.GetComponent<UIAnimatedImage> ().reset ();
		velaIconRI.texture = velaApagadaTex;
		refreshStoryStart_cd = 5;



		//buhoScaler.Start ();
		//buhoScaler.reset ();
	}

	// Use this for initialization
	void Start () {
	
		execute (this, "initScreen");
//		execute (canvasHub.screenCCanvas, "SetActive", true);
//		execute (screenCDrawHide, "Start");
//		execute (screenCDrawHide, "reset");
//		waitForTask (screenCDrawHide, "show");
		//execute (canvasHub.screenFCanvas, "SetActive", false);

//		createSubprogram ("candleHit");
//		delay (1.0f);
		waitForProgram (controllerHub.screenKController);
		programNotifyFinish ();

	}
	
	// UI callbacks
	public void clickOnVela() {
		
			goTo ("candleHit");
			
	}

	public void changeStoryStarting() {

		if (refreshStoryStart_cd > 0) {
			int newRow = storyStartTable.getNextRowIndex ();
			while (newRow == storyIndex) {
				newRow = storyStartTable.getNextRowIndex ();
			}
			storyIndex = newRow;
			storyStartText.text = (string)storyStartTable.getElement (0, newRow);
			storyStarts.Add (newRow);
			--refreshStoryStart_cd;
			storyStartsIndex = 0;
		} else {
			int newRow = storyStarts [storyStartsIndex];
			storyStartText.text = (string)storyStartTable.getElement (0, newRow);
			storyStartsIndex = (storyStartsIndex + 1) % storyStarts.Count;
		}

	}
}
