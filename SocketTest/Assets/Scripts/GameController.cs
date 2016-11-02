using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private SocketController socketController;

	// Use this for initialization
	void Start () {
		socketController = new SocketController ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp ("s")) {
			socketController.Send (socketController.getData());
		}
	}

	void OnDestroy () {
		if (socketController != null) {
			socketController.Close ();
			socketController = null;
		}
	}
}
