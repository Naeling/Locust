using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerIntroScreen : MonoBehaviour {

	public Intro intro;

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player"){
			intro.IntroScreen();
		}
	}
}
