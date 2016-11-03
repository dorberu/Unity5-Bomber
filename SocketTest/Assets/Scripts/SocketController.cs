using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net;
using MsgPack.Serialization;

public class SocketController {

	private GameController _gameController;
	WebSocket ws;

	private bool _close = false;
	private bool _error = false;

	// Use this for initialization
	public SocketController (GameController gameController) {
		_gameController = gameController;

//		ws = new WebSocket ("ws://192.168.11.4:8080");
		ws = new WebSocket ("ws://www3415uo.sakura.ne.jp:8080");

		ws.OnOpen += (sender, e) => {
			Debug.Log ("WebSocket Open");
			_gameController.connectionStatus ["Player"] = true;
		};

		ws.OnMessage += (sender, e) => {
			Dictionary<string, string> data = Deserialize (e.RawData);
			_gameController.packetReceive (data);
		};

		ws.OnError += (sender, e) => {
			Debug.Log ("Error Message: " + e.Message);
			_error = true;
		};

		ws.OnClose += (sender, e) => {
			Debug.Log ("WebSocket Close: [" + e.Code + "] " + e.Reason);
			_close = true;
		};

		ws.Log.Level = LogLevel.Trace;
		ws.Connect ();
	}

	public void Send (Dictionary<string, string> data) {
		if (! isConnect()) {
			return;
		}
		byte[] message = Serialize (data);
		ws.Send (message);
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

	public bool isConnect () {
		if (_close || _error) {
			return false;
		}
		return true;
	}
}
