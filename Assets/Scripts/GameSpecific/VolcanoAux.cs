using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoAux : MonoBehaviour, ButtonPressListener {

	public ControllerHub controllerHub;

	public void buttonPress() {
		if (controllerHub.screenAController.block)
			return;
		controllerHub.finishGameController.openVotationPanel ();
		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("openvotationpanel"));
		controllerHub.screenXController.winHero = controllerHub.gameController.gameState.turnPlayer;
		//controllerHub.screenXController.winnerHero.texture = controllerHub.screenXController.heroes [controllerHub.gameController.gameState.turnPlayer];
		controllerHub.networkController.broadcast (FGNetworkManager.makeClientCommand ("winner"));
		controllerHub.screenAController.block = true;
	}
}
