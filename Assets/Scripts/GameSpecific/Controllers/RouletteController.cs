using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RouletteType { TestType, ChoosePlayer };

public class RouletteController : FGProgram {

//	public GameController gameController;
//	public MasterController masterController;
//	public MainGameController mainGameController;
	public ControllerHub controllerHub;

	public AudioClip rouletteTick;

	//public GameObject wheel;
	public GameObject arrow;
	//public UIOpacityWiggle wheelSelection;

	public float maxAngSpeed = -20.0f;

	public float angSpeed;
	float angle;
	public float angAccel = 2.0f;
	const float SpeedThreshold = 0.1f;
	public float clampedAngle;
	public float clampledAngle_5;

	public int selectedItem = -1;
	public int selectedTestType = -1;

	public RouletteType rouletteType = RouletteType.TestType;

	float initialSpeedSign;

	public int lastType = -1;
	public int sameTypeTimes = 0;
	public int maxTimesSameType = 2;

	const float delay = 3.0f;

	int state = 0;
	float _timer;

	float timeToStartBraking;
	const float minBrakingTime = 0.2f;
	const float maxBrakingTime = 5.0f;

	float finishAngle;
	const float minFinishAngle = 360.0f * 3.0f;
	const float maxFinishAngle = 360.0f * 5.0f;

	float T;

	bool hasTicked = false;

	bool isSpinning = false;
	bool hasFinishedSpinning = false;

	bool rouletteCanSpin = true; // prevent a second button push


	public void initialize() {
		angSpeed = 0.0f;
		angle = 0.0f;
		state = 0;
		_timer = 0;
		hasFinishedSpinning = false;
		isSpinning = false;
	}



	// this is executed by non-turn players only
	public void setFinishAngle(float fa) {
		
		finishAngle = fa;

		selectedItem = 5 - (int)Mathf.Floor ((finishAngle - Mathf.Floor (finishAngle / 360.0f) * 360.0f) / 60.0f);

		if (rouletteType == RouletteType.TestType) {
			selectedTestType = selectedItem % 2;
		} 

		T = Mathf.Sqrt ((2 * finishAngle) / angAccel);
		angle = finishAngle - 0.5f * angAccel * (T) * (T);
		this.transform.localRotation = Quaternion.Euler (0, 0, -angle);


	}

	// this is executed on turn player only
	public float decideAngle() {

_ohMyGodICantBelieveIUsedAGoto:

		bool finish = false;

		while (!finish) {
			finishAngle = Random.Range (minFinishAngle, maxFinishAngle);
			float cAngle = finishAngle - Mathf.Floor (finishAngle / (360.0f / 6.0f)) * (360.0f / 6.0f);
			while ((cAngle + 5.0f) < 16.0f) {
				finishAngle = Random.Range (minFinishAngle, maxFinishAngle);
				cAngle = finishAngle - Mathf.Floor (finishAngle / (360.0f / 6.0f)) * (360.0f / 6.0f);
			}

			selectedItem = 5 - (int)Mathf.Floor ((finishAngle - Mathf.Floor (finishAngle / 360.0f) * 360.0f) / 60.0f);
			if (rouletteType == RouletteType.TestType) {
				finish = true;
				selectedTestType = selectedItem % 2;
				if (selectedTestType == controllerHub.lastTypeController.lastType) {
					++controllerHub.lastTypeController.sameTypeTimes;
					if (controllerHub.lastTypeController.sameTypeTimes >= maxTimesSameType) {
						goto _ohMyGodICantBelieveIUsedAGoto;
					}
				} else {
					controllerHub.lastTypeController.sameTypeTimes = 0;
				}
				controllerHub.lastTypeController.lastType = selectedTestType;

			} else {
				finish = controllerHub.gameController.gameState.playerList[selectedItem].isPresent;
			}
		}

		T = Mathf.Sqrt ((2 * finishAngle) / angAccel);
		angle = finishAngle - 0.5f * angAccel * (T) * (T);
		this.transform.localRotation = Quaternion.Euler (0, 0, -angle);

		return finishAngle;
		//wheelSelection.reset ();
	}

	public float decideAngleFromList(List<int> playingPlayers) {

		bool finish = false;
		bool weAreDone = true;
		for (int i = 0; i < playingPlayers.Count; ++i) {
			if (playingPlayers [i] != -1) {
				weAreDone = false; 
				break;
			}
		}
		if (weAreDone)
			return -1.0f;

		while (!finish) {
			finishAngle = Random.Range (minFinishAngle, maxFinishAngle);
			float cAngle = finishAngle - Mathf.Floor (finishAngle / (360.0f / 6.0f)) * (360.0f / 6.0f);
			while ((cAngle + 5.0f) < 16.0f) {
				finishAngle = Random.Range (minFinishAngle, maxFinishAngle);
				cAngle = finishAngle - Mathf.Floor (finishAngle / (360.0f / 6.0f)) * (360.0f / 6.0f);
			}

			selectedItem = 5 - (int)Mathf.Floor ((finishAngle - Mathf.Floor (finishAngle / 360.0f) * 360.0f) / 60.0f);
			if (rouletteType == RouletteType.TestType) {
				finish = true;
				selectedTestType = selectedItem % 2;
			} else {
				finish = playingPlayers.Contains (selectedItem);
			}
		}

		T = Mathf.Sqrt ((2 * finishAngle) / angAccel);
		angle = finishAngle - 0.5f * angAccel * (T) * (T);
		this.transform.localRotation = Quaternion.Euler (0, 0, -angle);

		return finishAngle;
		//wheelSelection.reset ();
	}
		

	public void go() {
		isSpinning = true;
		_timer = 0.0f;
		hasFinishedSpinning = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (!isSpinning)
			return;


		if (!hasFinishedSpinning) { // spinning the wheel
			// do a very simple wheel dynamics update 
			//  (using a 2d rigid body would be overkill, I'm afraid)

			if (_timer < T) {
				angle = finishAngle - 0.5f * angAccel * (T - _timer) * (T - _timer);
				_timer += Time.deltaTime;
			} 
			else {
				angle = finishAngle;
				_timer = 0.0f;

				//selectedItem = 5-(int)Mathf.Floor ((angle - Mathf.Floor (angle / 360.0f) * 360.0f) / 60.0f);

				//if(
				selectedTestType = selectedItem % 2; 

				//gameController.selectedItem = selectedItem;
				if (rouletteType == RouletteType.TestType) {
					controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("wheelitem", selectedItem));
				}


				hasFinishedSpinning = true;

				notifyFinish ();

			}



			clampedAngle = angle - Mathf.Floor (angle / (360.0f / 6.0f)) * (360.0f / 6.0f);


			if ((clampedAngle + 5.0f) < 16.0f) {
				if (!hasTicked) {
					controllerHub.audioController.playSound (rouletteTick);
					hasTicked = true;
				}
				if(arrow!=null)
				arrow.transform.localRotation = Quaternion.Euler (0, 0, 6.0f * (clampedAngle + 5.0f));
			} else {
				hasTicked = false;
				if(arrow!=null)
				arrow.transform.localRotation = Quaternion.Euler (0, 0, 0);
			}

			this.transform.localRotation = Quaternion.Euler (0, 0, -angle);

		}


	}
}
