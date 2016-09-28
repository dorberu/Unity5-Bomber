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
			Debug.Log ("WebSocket Message: " + System.Text.Encoding.UTF8.GetString(e.RawData));
		};

		ws.OnError += (sender, e) => {
			Debug.Log ("WebSocket Error Message: " + e.Message);
		};

		ws.OnClose += (sender, e) => {
			Debug.Log ("WebSocket Close: [" + e.Code + "] " + e.Reason);
		};

		ws.Log.Level = LogLevel.Trace;
		ws.Connect ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp ("s")) {
			Debug.Log ("WebSocket Send Message");
			ws.Send ("Send Test");
		}
	}

	void OnDestroy () {
		ws.Close ();
		ws = null;
	}
}
