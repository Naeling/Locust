using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public GameManager gameManager;

	void OnTriggerEnter (Collider other){
		if (other.CompareTag("Player")){
            // Call a method to save the time
            gameManager.SaveTime();
            gameManager.Restart();
		}
	}
}
