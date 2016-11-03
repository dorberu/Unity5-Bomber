using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	private SocketController socketController;
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Dictionary<string, bool> connectionStatus;
	public Dictionary<string, Vector3> updatePosition;
	public Dictionary<string, Vector3> updateDirection;
	private GameObject _player;
	private Dictionary<string, GameObject> _enemies;

	private long _positionSendFrame = 0;

	// Use this for initialization
	void Start () {
		connectionStatus = new Dictionary<string, bool> ();
		updatePosition = new Dictionary<string, Vector3> ();
		updateDirection = new Dictionary<string, Vector3> ();
		_enemies = new Dictionary<string, GameObject> ();
		socketController = new SocketController (this);
	}
	
	// Update is called once per frame
	void Update () {
		updateConnection ();
		updatePlayer ();
		updateEnemy ();
	}

	void OnDestroy () {
		if (socketController != null) {
			socketController.Close ();
			socketController = null;
		}
	}

	private void updateConnection () {
		foreach (KeyValuePair<string, bool> status in connectionStatus) {
			if (status.Value == false) {
				continue;
			}
			if (status.Key == "Player" && _player == null) {
				entryPlayer ();
			}
			if (status.Key != "Player" && ! _enemies.ContainsKey (status.Key)) {
				entryEnemy (status.Key);
			}
		}
	}

	private void entryPlayer () {
		_player = Instantiate (playerPrefab) as GameObject;
		Dictionary<string, string> data = new Dictionary<string, string> ();
		data["id"] = "2";
		data["position"] = vector3ToStr (_player.transform.position);
		socketController.Send (data);
	}

	private void entryEnemy (string handlerId) {
		_enemies [handlerId] = Instantiate (enemyPrefab) as GameObject;
	}

	private void updatePlayer () {
		if (++_positionSendFrame >= 5) {
			if (_player != null) {
				Dictionary<string, string> data = new Dictionary<string, string> ();
				data["id"] = "3";
				data["position"] = vector3ToStr (_player.transform.position);
				data["direction"] = vector3ToStr (_player.GetComponent<Player> ().direction);
				socketController.Send (data);
			}
			_positionSendFrame = 0;
		}
	}

	private void updateEnemy () {
		foreach (KeyValuePair<string, Vector3> position in updatePosition) {
			if (_enemies.ContainsKey (position.Key)) {
				_enemies [position.Key].GetComponent<Enemy> ().updatePosition = position.Value;
			}
		}
		foreach (KeyValuePair<string, Vector3> direction in updateDirection) {
			if (_enemies.ContainsKey (direction.Key)) {
				_enemies [direction.Key].GetComponent<Enemy> ().updateDirection = direction.Value;
			}
		}
	}

	public void packetReceive (Dictionary<string, string> data) {
		string handlerId = "";
		switch (data ["id"]) {
		case "1":
//			Debug.Log ("heartBeat Recevied.");
			break;
		case "2":
			// 他プレイヤーのログイン
			handlerId = data ["handlerId"];
			if (! _enemies.ContainsKey (handlerId)) {
				connectionStatus [handlerId] = true;
			}
			break;
		case "3":
			// 移動
			handlerId = data ["handlerId"];
			if (! _enemies.ContainsKey (handlerId)) {
				connectionStatus [handlerId] = true;
				return;
			}
			updatePosition [handlerId] = strToVector3 (data ["position"]);
			updateDirection [handlerId] = strToVector3 (data ["direction"]);
			break;
		}
	}

	private string vector3ToStr (Vector3 vector3) {
		return vector3.x + "," + vector3.y + "," + vector3.z;
	}

	private Vector3 strToVector3 (string str) {
		string[] strArrayData = str.Split (',');
		if (strArrayData.Length != 3) {
			return Vector3.zero;
		}
		float x = float.Parse (strArrayData[0]);
		float y = float.Parse (strArrayData[1]);
		float z = float.Parse (strArrayData[2]);
		return new Vector3 (x, y, z);
	}
}
