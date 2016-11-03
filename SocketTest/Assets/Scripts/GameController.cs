using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private SocketController socketController;
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	// Use this for initialization
	void Start () {
		socketController = new SocketController (this);
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

	public void entryPlayer () {
		Instantiate (playerPrefab);
	}
}
