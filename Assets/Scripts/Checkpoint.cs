using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public GameManager gameManager;

	// Register the checkpoint reached
	void OntriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			gameManager.checkpointReached = true;
		}
	}
}
