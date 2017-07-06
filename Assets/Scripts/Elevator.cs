using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elevator : MonoBehaviour {

	public String direction;
	private Vector3 translationDirection;
	public float distance;
	public float speed;
	public Boolean isMoving;
	private Vector3 targetPosition;
	private Vector3 startPosition;
	private Boolean isUp;



	// Use this for initialization
	void Start () {
		if (direction == "forward") {
			translationDirection = transform.forward;
		} else if (direction == "right") {
			translationDirection = transform.right;
		} else if (direction == "up") {
			translationDirection = transform.up;
		} else {
			translationDirection = transform.forward;
		}

		startPosition = transform.position;
		targetPosition = transform.position + translationDirection * distance;

		isUp = true;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (isUp){
			transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.fixedDeltaTime * speed);
		} else {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * speed);
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player"){
			StartCoroutine("Move");
		}
	}

	public void ResetPosition(){
		isUp = true;
	}

	IEnumerator Move(){
		yield return new WaitForSeconds(2);
		isUp = !isUp;
	}

}
