using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;

public interface FGNetworkSendIndicator {

	void signalSend();

}

public interface FGNetworkCommandProcessor {

	void processCommand(string comm);

}

public interface FGNetworkAppPauseCallback {

	void pauseNetwork();
	void resumeNetwork();

}

[System.Serializable]
public class EnqueuedMessage {

	public int seq;
	public string dest;
	public string fullMessage;

}

public class FGNetworkManager : MonoBehaviour {

	public Dictionary <string, int> receiveSeq;
	public List<string> seenOrigins;
	public Dictionary <string, int> sendSeq;
	public List<EnqueuedMessage> sendList;

	public UIEnableImageOnTimeout noConnectionTimeout;

//	[HideInInspector]
//	public string bigLog = "";

	protected FGNetworkCommandProcessor commandProcessor;
	protected FGNetworkAppPauseCallback pauseCallback;
	protected FGNetworkSendIndicator sendIndicator;

	bool connected = false;

	public Queue<string> commandQueue;
	public List<string> commandHistory;

	bool initialized = false;


//	public int connectionId;
//	public int hostId;
//	public int reliableChannel;
	public int bytesRead;

	protected TcpClient tcpClient;
	NetworkStream ns;
	//SslStream sslNs;

	StreamWriter sw; 

	string GameServerURL;
	int GameServerPort;

	protected Thread marcoPoloThread;
	protected Thread readThread;
	public bool isMarcoThreadRunning = false;
	public bool isThreadRunning = false;
	bool dataAvailable = false;
	string readData;
	const int poloMs = 5000;
	public float poloElapsedTime;
	public const float poloTimeout = 15.0f;
	public bool tryingToReconnect = false;
	protected float reconnectElapsedTime = 0.0f;
	protected const float reconnectRetry = 4.0f;


	public string localUserLogin;

	byte[] bytes;

	[HideInInspector]
	public bool firstConnectionStablished = false;


	public void initialize(string serverURL, int port) {

		if (initialized)
			return;

		//commandList = new List<string> ();
		commandQueue = new Queue<string> ();
		commandHistory = new List<string> ();

		GameServerURL = serverURL;
		GameServerPort = port;
		bytes = new byte[1024];

		isThreadRunning = false;
		isMarcoThreadRunning = false;



		initialized = true;

	}

	public int connect() {
		return connect (GameServerURL, GameServerPort);

	}

	public string consumeData() {

		string res;
		res = commandQueue.Dequeue ();
		return res;

	}

	public void marcoThreadCycle() {
		while (isMarcoThreadRunning) {
			Thread.Sleep (poloMs);
			sw.WriteLine ("marco");
			//sendIndicator.signalSend ();

			// resend all enqueued messages
			for (int i = 0; i < sendList.Count; ++i) {
				sendMessage (sendList[i].fullMessage);
			}

			//sw.Flush ();
		}
	}

	public void threadCycle() {

		while (isThreadRunning) {

			if (ns != null) {
				bytesRead = ns.Read (bytes, 0, bytes.Length);
				//int bytesRead = sslNs.Read(bytes, 0, bytes.Length);
				if (bytesRead > 0) {

					bytes [bytesRead] = 0;
					string newData = System.Text.Encoding.UTF8.GetString (bytes);
					//if (newData.EndsWith ("\\n"))
					newData = newData.Substring (0, bytesRead);

					//vomitNetworkOutput.text = newData;
					//commandList.Add (newData);

//					bigLog += (System.DateTime.Now.ToString() + " " + newData);

					commandQueue.Enqueue (newData);
					commandHistory.Add (newData);

				} else {
				
					isThreadRunning = false;

				}
			} else {
				
			}

		}
	}

	void OnDestroy() {
		isThreadRunning = false;
	}

	void OnApplicationPause( bool pauseStatus )
	{

		if (pauseStatus == true) {

			if (pauseCallback != null)
			pauseCallback.pauseNetwork ();

		}


		else if (pauseStatus == false) { // returning from pause

			if(pauseCallback != null)
			pauseCallback.resumeNetwork ();


		}


	}

	public void disconnectGently() {
		
		if (tcpClient != null)
			tcpClient.Close ();
		connected = false;
	}

	/*
	 * Disconnect from the server!
	 */
	public void disconnect() {
		isThreadRunning = false;
		isMarcoThreadRunning = false;
		if(marcoPoloThread != null)
		marcoPoloThread.Join ();
		if (readThread != null)
		readThread.Join ();
		if(ns != null)
		ns.Close ();
		if (tcpClient != null)
		tcpClient.Close ();
		connected = false;
		//noConnectionTimeout.stop ();
	}


	// connect method: can connect either to the wisdomini.flygames.org relay
	// or directly to another user??
	public int connect(string url, int port) {

//		GameServerPort = port;
//		GameServerURL = url;

		initialize (url, port);

		int result;
		try {
			tcpClient = new TcpClient (url, port);
			result = 0;
		}
		catch(SocketException e) {
			result = -1;
			return result;
		}

		X509Certificate2 clientCertificate = new X509Certificate2();
		X509Certificate2[] cerCol = { clientCertificate };
		X509CertificateCollection clientCertificateCollection = new X509Certificate2Collection (cerCol);
		//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

		ns = tcpClient.GetStream ();
		/*
		sslNs = new SslStream (tcpClient.GetStream (), true, new RemoteCertificateValidationCallback
			(
				(srvPoint, certificate, chain, errors) => true//MyRemoteCertificateValidationCallback(srvPoint, certificate, chain, errors)
			));
		sslNs.AuthenticateAsClient (FGUtils.flygamesSSLAuthHost, clientCertificateCollection, SslProtocols.Tls, true);//, clientCertificateCollection, SslProtocols.Tls, false);
		//System.Net.ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors ) => true;
		System.Net.ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
			(
				(srvPoint, certificate, chain, errors) => true//MyRemoteCertificateValidationCallback(srvPoint, certificate, chain, errors)
			);

		*/
		ns.ReadTimeout = Timeout.Infinite;
		sw = new StreamWriter (ns) {

			AutoFlush = true

		};
		sw.AutoFlush = true;
		readThread = new Thread (threadCycle); // create a new thread
		marcoPoloThread = new Thread(marcoThreadCycle); // create another thread
		isThreadRunning = true; // make the thread loop go!
		isMarcoThreadRunning = true; // set marco thread running too!
		try {

			readThread.Start ();

		}
		catch(ThreadStartException e) {
			// do nothing, really

		}
		try {
			marcoPoloThread.Start();
		}
		catch(ThreadStartException e) {
			
		}

		//byte error;
		//connectionId = NetworkTransport.Connect(hostId, Utils.GameRelayServer, Utils.GameRelayPort, 0, out error);

		//return error;
		if (result == 0) {
			connected = true;
			if(noConnectionTimeout != null) {
				noConnectionTimeout.go ();
			}
		}
		return result;

	}

	public int connectGently(string url, int port) {

		initialize (url, port);

		int result;
		try {
			tcpClient = new TcpClient (url, port);
			result = 0;
		}
		catch(SocketException e) {
			result = -1;
			return result;
		}


		if (result == 0) {
			connected = true;

		}
		return result;

	}

	public void idPlayer(string id) {
		sendMessage ("id " + id);

	}

	public void makeRoom(string roomname) {
		sendMessage ("makeroom " + roomname);
	}

	public void joinRoom(string roomname) {
		sendMessage ("joinroom " + roomname);
	}

	public void unseeOrigin(string o) {
		if(receiveSeq.ContainsKey(o)) {
			receiveSeq.Remove (o);
		}
		if (sendSeq.ContainsKey (o)) {
			sendSeq.Remove (o);
		}
	}

	public int receiveSeqFor(string origin) {

		if(!seenOrigins.Contains(origin)) { // WARNING REMOVE
			seenOrigins.Add (origin);
		}

//		if (origin.Equals ("")) // prevent this (PATCH)
//			return 0;
		if((origin.IndexOf("@")) == -1) {
			
		}


		if (receiveSeq.ContainsKey (origin)) {
			return receiveSeq [origin];
		} else {
			receiveSeq [origin] = 0;
			return 0;
		}

	}

	public int sendSeqFor(string dest) {

		if (sendSeq.ContainsKey (dest)) {
			return sendSeq [dest];
		} else {
			sendSeq [dest] = 0;
			return 0;
		}

	}

	public void incReceiveSeqFor(string origin) {
		receiveSeq [origin]++;
	}

	public void incSendSeqFor(string dest) {
		sendSeq [dest]++;
	}

	// remove from sendlist once message has been acknowledged
	public void ack(int seq, string origin) {
		int rSeq; 
		rSeq = receiveSeqFor (origin);
		for (int i = 0; i < sendList.Count; ++i) {
			EnqueuedMessage msg = sendList [i];
			if ((msg.seq == seq) && (msg.dest.Equals (origin))) { 
				sendList.RemoveAt (i);
				--i;
			}
		}
	}

	public void sendCommand(string recipient, string command) {

		if (recipient.IndexOf('$') != -1)
			recipient = recipient.Substring (0, recipient.IndexOf('$'));
		int seq = sendSeqFor (recipient);
		string safeCommand = seq + "#" + localUserLogin + "#" + command;
		string fullMessage = "sendmessage " + recipient + " " + safeCommand;

		sendMessage (fullMessage);
		incSendSeqFor (recipient);
		EnqueuedMessage newMessage = new EnqueuedMessage ();
		newMessage.seq = seq;
		newMessage.dest = recipient;
		newMessage.fullMessage = fullMessage;
		sendList.Add (newMessage);


	}

	public void sendCommandUnsafe(string recipient, string command) {

		sendMessage ("sendmessage " + recipient + " " + command);

	}

	public void sendMessage(string command) {

		if (sw == null)
			return;
		sw.WriteLine (command + "$"); // $ is end of command

		//sw.Flush ();

	}

	public void broadcast(string command) {

		// a bunch of individual sends
		foreach(var dest in receiveSeq.Keys)
		{

			if((!dest.Equals(localUserLogin)) && (!dest.Equals(""))) { // do not self-send

				sendCommand (dest, command);

			}
		}

	}

	public void broadcastUnsafe(string command) {
		sendMessage ("broadcast " + command);
	}

//	public void broadcast(string command, int g) {
//		sendMessage ("groupbroadcast " + command + " " + g);
//	}

	/*public void sendMessage(string to, string message) {

	}*/

	// Use this for initialization
	void Start () {
		sw = null;
	}

	// Update is called once per frame
	protected void Update () {

		if (!initialized)
			return;

		if(connected) poloElapsedTime += Time.deltaTime;

		while (commandQueue.Count > 0) {


			string command = consumeData ();
			commandProcessor.processCommand(command);


		}

	}

	public static string makeClientCommand(params object[] arg) {

		string res = "";
		for (int i = 0; i < arg.Length; ++i) {
			res += (arg [i].ToString() + ":");
		}
		return res;

	}


}

