using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenIController : FGProgram {

	public ControllerHub controllerHub;
	public CanvasHub canvasHub;

	public Texture velaApagadaTex;
	public RawImage velaIconRI;

	public RosettaWrapper rosettaWrapper;
	public FGTable sciencePhrases;
	public FGTable mindPhrases;

	public Text storyStartText;
	public FGTable storyBeginTable;

	public Text phraseText;
	List<int> nullCasePhrases;

	public UIDrawHide drawIHide;

	int refreshStoryStart_cd = 2;
	int storyStartsIndex = 0;
	List<int> storyStarts;

	public int phraseIndex = 0;

	int storyIndex = 0;

	int nullCasePhraseIndex = 0;

	public int phraseLength = 0;

	private void updatePhraseText() {
		int typedPhraseType = controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].phrasesObtained [phraseIndex].type;
		int typedPhraseIndex = controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].phrasesObtained [phraseIndex].index;
		if (typedPhraseIndex == GameController.Science) {
			phraseText.text = ((string)sciencePhrases.getElement(0, typedPhraseIndex)).Replace ("_", "");
		}
		else {
			phraseText.text = ((string)mindPhrases.getElement(0, typedPhraseIndex)).Replace ("_", "");
		}
	}

	public void initializeScreen() {

		phraseLength = controllerHub.gameController.gameState.playerList [controllerHub.gameController.gameState.localNPlayer].phrasesObtained.Count;
//		if (phraseLength > 0) {
//			phraseIndex = Random.Range (0, phraseLength);
//			updatePhraseText ();
//		} else {
			nullCasePhraseIndex = 0;
			nullCasePhrases = new List<int> ();
			int phrase1 = Random.Range (0, mindPhrases.nRows ()); // antes era una de ciencia y otra de mente
			int phrase2 = Random.Range (0, mindPhrases.nRows ()); // ahora sólo de mente
		while (phrase2 == phrase1) {
			phrase2 = Random.Range (0, mindPhrases.nRows ());
		}
			nullCasePhrases.Add (phrase1);
			nullCasePhrases.Add (phrase2);
			phraseText.text = ((string)mindPhrases.getElement(0, phrase1)).Replace ("_", "");
		//}
		velaIconRI.gameObject.GetComponent<UIAnimatedImage> ().Start ();
		velaIconRI.gameObject.GetComponent<UIAnimatedImage> ().reset ();
		velaIconRI.texture = velaApagadaTex;
		refreshStoryStart_cd = 2;
		storyStarts = new List<int> ();
		int nextRow = storyBeginTable.getNextRowIndex ();
		storyStartText.text = (string)storyBeginTable.getElement(0, nextRow);
		storyIndex = nextRow;
		storyStarts.Add (nextRow);
		drawIHide.Start ();

	}

	public void scrollLeft() {

//		if (phraseLength > 0) {
//			phraseIndex = (phraseIndex + phraseLength - 1) % phraseLength;
//			updatePhraseText ();
//		} else {
			if (nullCasePhraseIndex == 0) {
				nullCasePhraseIndex = 1;
				phraseText.text = ((string)mindPhrases.getElement(0, nullCasePhrases[1])).Replace ("_", "");
			}
			else {
				nullCasePhraseIndex = 0;
				phraseText.text = ((string)mindPhrases.getElement(0, nullCasePhrases[0])).Replace ("_", "");
			}
		//}

	}

	public void scrollRight() {


//		if (phraseLength > 0) {
//			phraseIndex = (phraseIndex + 1) % phraseLength;
//			updatePhraseText ();
//		}
//		else {
			if (nullCasePhraseIndex == 0) {
				nullCasePhraseIndex = 1;
				phraseText.text = ((string)mindPhrases.getElement(0, nullCasePhrases[1])).Replace ("_", "");
			}
			else {
				nullCasePhraseIndex = 0;
				phraseText.text = ((string)mindPhrases.getElement(0, nullCasePhrases[0])).Replace ("_", "");
			}
		//}
	}


	// Use this for initialization
	void Start () {

		execute (this, "initializeScreen");

		execute (canvasHub.screenICanvas, "SetActive", true);
		execute (drawIHide, "Start");
		execute (drawIHide, "hideImmediately");
		waitForTask (drawIHide, "show");
		execute (canvasHub.screenECanvas, "SetActive", true);
		delay (1.0f);

		createSubprogram ("SandClock");
		delay (1.0f);
		waitForProgram (controllerHub.screenZController);

		programNotifyFinish ();
		
	}

	public void clickOnVela() {

		goTo ("SandClock");

	}


	public void changeStoryStarting() {

		if (refreshStoryStart_cd > 0) {
			int newStarting = Random.Range (0, storyBeginTable.nRows ());//storyStartSBLength);
			while (newStarting == storyIndex) {
				newStarting = Random.Range (0, storyBeginTable.nRows ());
			}
			storyStarts.Add (newStarting);
			storyIndex = newStarting;
			storyStartText.text = (string)storyBeginTable.getElement (0, storyIndex);
			--refreshStoryStart_cd;
			storyStartsIndex = 0;
		}
		 else {
			int newRow = storyStarts [storyStartsIndex];
			storyStartText.text = (string)storyBeginTable.getElement (0, newRow);
			storyStartsIndex = (storyStartsIndex + 1) % storyStarts.Count;
		}

	}

}
