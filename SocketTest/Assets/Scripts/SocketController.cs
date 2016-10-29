using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net;
using MsgPack.Serialization;

public class SocketController : MonoBehaviour {

	WebSocket ws;

	String connectCount;
	GameObject connectionText;

	// Use this for initialization
	void Start () {
		ws = new WebSocket ("ws://192.168.11.4:8080");
		this.connectionText = GameObject.Find ("connectionText");

		ws.OnOpen += (sender, e) => {
			Debug.Log ("WebSocket Open");
		};

		ws.OnMessage += (sender, e) => {
			Dictionary<string, string> data = Deserialize (e.RawData);
			if (data["id"] == "1")
			{
				this.connectCount = "connectCount: " + data["connectCount"];
			}
		};

		ws.OnError += (sender, e) => {
			Debug.Log ("Error Message: " + e.Message);
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
			Dictionary<string, string> data = getData ();
			Debug.Log ("Send Message: " + data["id"] + " : " + data["flag"] + " : " + data["日本語のキー"]);
			byte[] message = Serialize (data);
			ws.Send (message);
		}
		this.connectionText.GetComponent<Text> ().text = this.connectCount;
	}

	Dictionary<string, string> getData () {
		Dictionary<string, string> map = new Dictionary<string, string> ();
		map["id"] = "2";
		map["flag"] = "true";
		map["日本語のキー"] = "日本語の値：クライアント";
		return map;
	}

	byte[] Serialize (Dictionary<string, string> data) {
		MemoryStream stream = new MemoryStream ();
		var serializer = MessagePackSerializer.Get<Dictionary<string, string>> ();
		serializer.Pack (stream, data);
		byte[] ret = stream.GetBuffer ();
		return ret;
	}

	Dictionary<string, string> Deserialize (byte[] data) {
		MemoryStream stream = new MemoryStream (data);
		var serializer = MessagePackSerializer.Get<Dictionary<string, string>> ();
		Dictionary<string, string> ret = serializer.Unpack (stream);
		return ret;
	}

	void OnDestroy () {
		ws.Close ();
		ws = null;
	}
}
