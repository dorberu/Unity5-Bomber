using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float moveSpeed = 5f;
	public float rotationSpeed = 360f;

	public Vector3 updatePosition = Vector3.zero;
	public Vector3 updateDirection = Vector3.zero;

	CharacterController characterController;
	Animator animator;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		animator = GetComponentInChildren<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if (updatePosition != Vector3.zero) {
			transform.position = updatePosition;
			updatePosition = Vector3.zero;
		}
		Vector3 direction = updateDirection;

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
