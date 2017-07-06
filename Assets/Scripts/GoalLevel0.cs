using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalLevel0 : MonoBehaviour {

	public GameManager gameManager;


	void OnTriggerEnter (Collider other){
		//TODO launch an automatic movement towards the last room, and the last dialog here
		if (other.CompareTag("Player")){

			gameManager.NoControl();
			StartCoroutine("goToLevelEndTimer");
		}
	}

	IEnumerator goToLevelEndTimer(){
		yield return new WaitForSeconds(1);
		gameManager.isGoingToLevelEnd = true;
	}
}
