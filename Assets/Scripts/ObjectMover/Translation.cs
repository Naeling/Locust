using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Translation : ObjectSwitcher {

	private Vector3 translationDirection;
	public string direction;
	public float translationMultiplier;
	public float translationSpeedMultiplier;
	// Indicates if the Object is in the state true or false, default true
	private Boolean state;
	// The Object is going to translate between the two positions
	private Vector3 position1;
	private Vector3 position2;


	public void Start(){
		//translationDirection = transform.forward;
		if (direction == "forward") {
			translationDirection = transform.forward;
		} else if (direction == "right") {
			translationDirection = transform.right;
		} else if (direction == "up") {
			translationDirection = transform.up;
		} else {
			translationDirection = transform.forward;
		}
		state = true;

		// This part of the code should be ok. Just generate the point to reach based on the direction requested and the sign of the multiplier.
		// A positive multiplier generate a direction opposite to the a negative multiplier
		position1 = transform.position;
		position2 = position1 + translationDirection * translationMultiplier;
	}
	public void FixedUpdate(){
		// Translate towards the position1
		if(state)
			{
				transform.position = Vector3.MoveTowards(transform.position, position1, Time.deltaTime * translationSpeedMultiplier);
			}
		// Translate towards the position2
		else {
			transform.position = Vector3.MoveTowards(transform.position, position2, Time.deltaTime * translationSpeedMultiplier);
		}
	}

	public override void Switch(){
		// Do something
		state = !state;
	}

}
