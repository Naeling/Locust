using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour {


	private Vector3 startingPosition;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		startingPosition = transform.position;
		rb = GetComponent<Rigidbody>();
	}

	void Reset() {
		rb.useGravity = false;
		transform.position = startingPosition;
	}

	public void Fall() {
		rb.useGravity = true;
	}
}
