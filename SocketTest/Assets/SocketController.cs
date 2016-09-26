using UnityEngine;
using System;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;

public class SocketController : MonoBehaviour {

	WebSocket ws;

	// Use this for initialization
	void Start () {
		ws = new WebSocket ("ws://192.168.11.4:8080");

		ws.OnOpen += (sender, e) => {
			Debug.Log ("WebSocket Open");
		};

		ws.OnMessage += (sender, e) => {
			Debug.Log ("WebSocket Message Type: " + e.GetType () + ", Data: " + e.Data);
		};

		ws.OnError += (sender, e) => {
			Debug.Log ("WebSocket Error Message: " + e.Message);
		};

		ws.OnClose += (sender, e) => {
			Debug.Log ("WebSocket Close: [" + e.Code + "] " + e.Reason);
		};

		Debug.Log ("Start");
		ws.Log.Level = LogLevel.Trace;
		ws.Connect ();
		Debug.Log ("End");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp ("s")) {
			ws.Send ("Test Message");
		}
	}

	void OnApplicationQuit () {
		Debug.Log ("Quit");
		if (ws != null && ws.ReadyState == WebSocketState.Open) {
			ws.Close ();
			ws = null;
		}
	}
}
