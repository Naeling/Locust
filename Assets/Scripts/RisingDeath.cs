using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RisingDeath : Death {

	public String direction;
	private Vector3 translationDirection;
	public float distance;
	public float speed;
	public Boolean isMoving;
	private Vector3 targetPosition;
	private Vector3 startPosition;

	void Start(){
		isMoving = false;
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
		Debug.Log(startPosition);
		targetPosition = transform.position + translationDirection * distance;
		Debug.Log(targetPosition);
	}

	void FixedUpdate(){
		if (isMoving){
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * speed);
		}
	}

	public void Move(){
		isMoving = true;
	}

	public void ResetPosition(){
		transform.position = startPosition;
		isMoving = false;
	}
}
