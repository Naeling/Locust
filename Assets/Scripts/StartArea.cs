using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartArea : MonoBehaviour {

	public Timer timer;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	// Stop the timer
	// void OnTriggerEnter(Collider other){
	// 	timer.StopTimer();
	// }
	// Launch the timer
	void OnTriggerExit(Collider other){
		timer.Go();
	}
}
