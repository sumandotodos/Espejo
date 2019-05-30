using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : FGNetworkManager, FGNetworkCommandProcessor, FGNetworkAppPauseCallback, FGNetworkSendIndicator {

	public BootStrapData bootstrapData;



	public int commandsReceived = 0;

	public ControllerHub controllerHub;


	public Flash send;
	public Flash receive;

	public void signalSend() {
		send.flash ();
	}

	public void pauseNetwork() {
		sendMessage ("endinvalidate");
		disconnect ();
		while (tcpClient.Connected) {   } // OoO oh no, don't do it!!
	}

	public void resumeNetwork() {
		if (tcpClient == null)
			connect ();
		if (tcpClient.Connected == false) {
			isMarcoThreadRunning = false;
			isThreadRunning = false;
			marcoPoloThread.Join ();
			readThread.Join (); // wait for threads to finish
			//initGame();
			connectAndEnterRoom();
		}
	}



	public int sync = 0;

	public void resetSync() {
		sync = 0;
	}
	public void addSync() {
		++sync;
	}
	public int getSync() {
		return sync;
	}



	WWW www;
	WWWForm wwwForm;

	public void createHTTPRequest(string url, params string[] data) {

		WWWForm theForm = new WWWForm();
		for(int i = 0; i < data.Length/2; ++i) {
			theForm.AddField(data[i*2], data[i*2 + 1]);
		}
		if (data.Length == 0) {
			theForm.AddField ("dummy", "3.1416");
		}

		www = new WWW (url, theForm);

	}

	public void getFreshRoomID() {

		createHTTPRequest (bootstrapData.loginServer + FGUtils.GetFreshRoomID);

	}

	public void checkAppUser(string user, string passwd) {

		createHTTPRequest (bootstrapData.loginServer + ":" + bootstrapData.loginServerPort + FGUtils.CheckUserScript, "email", user, "passwd", passwd, "app", "EspLite");

	}

	public void connectAndEnterRoom() {
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		enterRoom ();
	}

	public void enterRoom() {
		controllerHub.gameController.gameState.localLogin = localUserLogin;
		sendMessage("initgame " + controllerHub.gameController.gameState.localLogin + " " + controllerHub.gameController.gameState.roomID);
	}

	public void initGame_withUser(string user) {
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		//sendMessage("initgame " + user + " " + controllerHub.gameController.gameState.roomID);
		sendMessage("roomuuid " + controllerHub.gameController.gameState.roomID);
	}

	public void initGame_withRoom(string room) {
		localUserLogin = "";
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		//sendMessage("initgame " + user + " " + controllerHub.gameController.gameState.roomID);
		sendMessage("roomuuid " + room);
	}

	public void initGame_void() {
		initGame ();
	}

	public void initGame() {
		localUserLogin = "";
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		//connect ();
		//sendMessage("initgame " + con trollerHub.gameController.gameState.localLogin + " " + controllerHub.gameController.gameState.roomID);

		sendMessage("roomuuid " + controllerHub.gameController.gameState.roomID);
	}

	public bool httpRequestIsDone() {

		return www.isDone;

	}

	public string wwwResult() {

		return www.text;

	}
		
	public bool localUserLoginSet() {
		return (!localUserLogin.Equals (""));
	}

	// Use this for initialization
	void Start () {

		localUserLogin = "";

		seenOrigins = new List<string> ();
		receiveSeq = new Dictionary<string, int> ();
		sendSeq = new Dictionary<string, int> ();
		sendList = new List<EnqueuedMessage> ();
		pauseCallback = this;
		commandProcessor = this;
		sendIndicator = this;

	}
	
	// Update is called once per frame
	new void Update () {
		
		if (tryingToReconnect) {
			reconnectElapsedTime += Time.deltaTime;
			if (reconnectElapsedTime > reconnectRetry) {
				reconnectElapsedTime = 0.0f;
				int res = connect ();
				if (res == 0) {
					tryingToReconnect = false;
					sendMessage ("initgame " + controllerHub.gameController.gameState.localLogin + " " + controllerHub.gameController.gameState.roomID);
				}
			}
		}

		if (poloElapsedTime > (poloTimeout)) {
			poloElapsedTime = 0.0f;
			disconnect ();
			tryingToReconnect = true;
			reconnectElapsedTime = reconnectRetry + 1.0f;
		}
		base.Update ();
	}

	int comN = 0;

	public void processCommand(string comm) {

		comm = comm.Replace ("\n", ""); // remove any \n coming down the stream, we don't want them

		string[] commands = comm.Split ('$'); // split back to back commands

		char[] charcomm = comm.ToCharArray ();
		int nCommands = 0;
		for (int i = 0; i < charcomm.Length; ++i) {
			if (charcomm [i] == '$')
				++nCommands;
		}

		commandsReceived += nCommands;

		for (int i = 0; i < nCommands; ++i) {

			string command = commands [i];

			command = command.Trim ('\n'); // remove all \n 's


			int safeIndex = command.IndexOf ('#');
			string[] safeArg;
			bool processThisCommand = true;
			bool safeCommand = false;
			int safeSeqNum = -1;
			string originOfSafeCommand = "";
			int expectedSeq = -1;

			if (safeIndex != -1) { // safe command
				safeCommand = true;
				safeArg = command.Split('#');
				int.TryParse (safeArg [0], out safeSeqNum);
				command = safeArg [2];
				originOfSafeCommand = safeArg [1];
				int fix = originOfSafeCommand.IndexOf ('$');
				if (fix != -1) {
					
					originOfSafeCommand = originOfSafeCommand.Substring (0, fix);
				}
				expectedSeq = receiveSeqFor (originOfSafeCommand);
				if (safeSeqNum != expectedSeq) {
					processThisCommand = false;
					//sendCommandUnsafe (originOfSafeCommand, "ACK:" + safeSeqNum + ":" + localUserLogin);
				}

			}

			if (processThisCommand) {


				string[] arg = command.Split (':');

				if (arg.Length > 0) {

					// We could replace this by a dictionary <string, Func> if we really wanted to

					// 
					if (command.StartsWith ("startgame")) {
						//controllerHub.masterController.AppDebug ("received command startgame:" + arg [1]);
						controllerHub.joinNewGameController.startGame (arg [1], arg[2], arg[3]);
					}
					if (command.StartsWith ("playerready")) {
						Debug.Log ("Player Ready: " + arg [1] + ", " + arg [2]);
						controllerHub.newGameController.addPlayer (arg[1], arg[2]);
					}
					if (command.StartsWith ("startmirrortest")) {
						controllerHub.screenAPrimeController.startMirrorTest ();
					}
					if (command.StartsWith ("startbooktest")) {
						controllerHub.screenAPrimeController.startBookTest (arg[1]);
					}
					if (command.StartsWith ("startwhitetest")) {
						controllerHub.screenAPrimeController.startWhiteTest (arg[1]);
					}
					if (command.StartsWith ("setnplayers")) {
						int np;
						int.TryParse (arg [1], out np);
						controllerHub.gameController.gameState.nPlayers = np;
						controllerHub.joinNewGameController.setNPlayers (np);
					}
					if (command.StartsWith ("whitequestion")) {
						int nq;
						int.TryParse (arg [1], out nq);
						bool sc;
						bool.TryParse (arg [2], out sc);
						controllerHub.screenDController.notMyTurnInitialization (nq, sc);
					}
					if (command.StartsWith ("presuntoanswer")) {
						int nq;
						int.TryParse (arg [1], out nq);
						controllerHub.screenDController.presuntoAnswer (nq);
					}
					if (command.StartsWith ("escreenangle")) {
						float an;
						float.TryParse (arg [1], out an);
						controllerHub.screenEController.setRouletteFinishAngle (an);
					}
					if (command.StartsWith ("finishactivity")) {
						controllerHub.screenAPrimeController.finishActivity ();
					}
					if (command.StartsWith ("lightmirror")) {
						controllerHub.screenDController.lightMirrorGlow ();
					}
					if (command.StartsWith ("veladecision")) {
						int pl;
						int.TryParse (arg [1], out pl);
						controllerHub.screenEController.incNumberOfDecisions (pl);
					}
					if (command.StartsWith ("openvotationpanel")) {
						controllerHub.finishGameController.openVotationPanel ();
					}
					if (command.StartsWith ("finishgame")) {
						controllerHub.mainGameLoopController.finishGame ();
					}
					if (command.StartsWith ("voteforfinishgame")) {
						controllerHub.finishGameController.voteForFinishGame ();
					}
					if (command.StartsWith ("cancelfinishgame")) {
						controllerHub.finishGameController.cancelFinishGame ();
					}
					if (command.StartsWith ("takeplayer")) {
						int np;
						int.TryParse (arg [1], out np);
						controllerHub.playerSelectController.takePlayer (np, arg [2]);
					}
					if (command.StartsWith ("roomuuid")) {
						if (!controllerHub.continueGameController.tryingToContinue) {
							Debug.Log ("Got user " + arg [1] + " for room " + controllerHub.gameController.gameState.roomID);
							localUserLogin = arg [1];
						}
					}
					if (command.StartsWith ("setcommonanswer")) {
						int ans;
						int.TryParse (arg [1], out ans);
						controllerHub.screenDController.setCommonAnswer (ans);
					}
					if (command.StartsWith ("answer")) {
						int pl;
						int type;
						int vote;
						int.TryParse (arg [1], out pl);
						int.TryParse (arg [2], out type);
						int.TryParse (arg [3], out vote);
						controllerHub.screenDController.receiveResponseFromOther (pl, type, vote);
					}
					if (command.StartsWith ("sync")) {
						controllerHub.gameController.addSyncPlayers ();
					}
					if (command.StartsWith ("nuke")) {
						controllerHub.masterController.nuke ();
					}
					if (command.StartsWith ("showkprime")) {
						int adv;
						int.TryParse (arg [1], out adv);
						controllerHub.screenEController.immediatelyFinishHWait = true; // small hack
						controllerHub.screenHWaitController.startKPrime (adv);
					}
					if (command.StartsWith ("updatelasttype")) {
						int t;
						int.TryParse (arg [1], out t);
						controllerHub.lastTypeController.updateLastType (t);
					}
					if (command.StartsWith ("winner")) {
						controllerHub.screenXController.winHero = controllerHub.gameController.gameState.turnPlayer;
					}
					if (command.StartsWith ("hwaitfinish")) {
						controllerHub.screenEController.immediatelyFinishHWait = true; // small hack
						controllerHub.screenHWaitController.finish ();
					}
					if (command.StartsWith ("donttakeplayer")) {
						controllerHub.playerSelectController.dontTakePlayer ();
					}
					if (command.StartsWith ("grabplayer")) {
						int pl;
						int.TryParse (arg [1], out pl);
						controllerHub.gameController.attemptGrabPlayer (pl, arg [2]);
					}
					if (command.StartsWith ("winwordgame")) {
						int pl;
						int.TryParse (arg [1], out pl);
						controllerHub.gameController.setWordGameFirstToWin (pl);
					}
					if (command.StartsWith ("reportwordgamewin")) {
						controllerHub.gameController.reportWordGameFirstToWin (arg [1]);
					}
					if (command.StartsWith ("declarebooknoplay")) {
						controllerHub.gameController.declareBookNoPlay ();
					}
					if (command.StartsWith ("reportcontinue")) {
						int ttl;
						int.TryParse (arg [3], out ttl);
						if (controllerHub != null) {
							controllerHub.continueGameController.ReportContinue (arg [1], arg [2], ttl);
						}

					}
					if (command.StartsWith ("firsttowin")) {
						int pl;
						int.TryParse (arg [1], out pl);
						if (controllerHub != null) {
							controllerHub.screenKController.setFirstToWinGame (pl);
						}
					}
					if (command.StartsWith ("roomplayers")) {

						int id = 1;
						while (!arg [id].Equals ("null")) {
							receiveSeqFor (arg [id]); // register roommate for broadcast

							id++;

						}

					}
					if (command.StartsWith ("ACK")) {
						
							//string[] fields = newData.Split (':');
							int sq;
							int.TryParse (arg [1], out sq);
							string fromUser = arg [2].TrimEnd ('$', '\n');
							ack (sq, fromUser);
						if (fromUser.IndexOf ('$') != -1) {
							
						}


					}
					if (command.StartsWith ("polo")) {
						//receive.flash ();
						if (noConnectionTimeout != null) {
							noConnectionTimeout.keepAlive ();
						}
						poloElapsedTime = 0.0f;
					}
					if (safeCommand) { // if safe command, acknowledge processing

						incReceiveSeqFor (originOfSafeCommand);
						sendCommandUnsafe (originOfSafeCommand, "ACK:" + safeSeqNum + ":" + localUserLogin);



					}

				}

			}

		}

	}
				
}
