using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static bool existsInstance = false;
	public float moveSpeed = 5f;
	public float rotationSpeed = 360f;

	public Vector3 direction = Vector3.zero;

	CharacterController characterController;
	Animator animator;

	void Awake () {
		if (existsInstance) {
			Destroy (gameObject);
			return;
		}
		existsInstance = true;
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		animator = GetComponentInChildren<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (direction.sqrMagnitude > 0.01f) {
			Vector3 forward = Vector3.Slerp(
				transform.forward,
				direction, 
				rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction)
			);
			transform.LookAt (transform.position + forward);
		}
		characterController.Move (direction * moveSpeed * Time.deltaTime);

		animator.SetFloat ("Speed", characterController.velocity.magnitude);
	}
}
