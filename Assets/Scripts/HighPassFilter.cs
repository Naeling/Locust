using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighPassFilter : MonoBehaviour {

	public AudioHighPassFilter audioFilter;
	public Rigidbody playerRigidbody;

	void Update(){
		Vector3 forwardSpeed = Vector3.Project(playerRigidbody.velocity, playerRigidbody.transform.forward);
		float speed = forwardSpeed.magnitude;
		audioFilter.cutoffFrequency = 1000f - 45 * speed;

	}


}
