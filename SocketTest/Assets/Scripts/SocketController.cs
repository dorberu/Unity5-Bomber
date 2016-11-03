using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net;
using MsgPack.Serialization;

public class SocketController {

	private GameController _gameController;
	WebSocket ws;

	// Use this for initialization
	public SocketController (GameController gameController) {
		_gameController = gameController;

		ws = new WebSocket ("ws://192.168.11.4:8080");

		ws.OnOpen += (sender, e) => {
			Debug.Log ("WebSocket Open");
			_gameController.entryPlayer ();
			_gameController.entryPlayer ();
		};

		ws.OnMessage += (sender, e) => {
			Dictionary<string, string> data = Deserialize (e.RawData);
			Debug.Log ("WebSocket Message: id=" +  data["id"]);
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

	public void Send (Dictionary<string, string> data) {
		String log = "";
		foreach (KeyValuePair<String, String> pair in data) { log += (pair.Key + "=" + pair.Value + "; "); }
		Debug.Log ("Send: " + log);
		byte[] message = Serialize (data);
		ws.Send (message);
	}

	public Dictionary<string, string> getData () {
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

	public void Close () {
		ws.Close ();
		ws = null;
	}
}
