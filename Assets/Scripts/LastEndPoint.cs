using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class LastEndPoint : MonoBehaviour {

	public Image image;
	public GameManager gameManager;


	// Use this for initialization
	void Start () {
		image.CrossFadeAlpha(0f, 6f, false);
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player"){
			image.CrossFadeAlpha(1f, 2f, false);
			gameManager.Immobilize();
			gameManager.endReached = true;
			StartCoroutine("TransitionTimer");
		}
	}

	IEnumerator TransitionTimer() {
		yield return new WaitForSeconds(2);
		gameManager.EndDialog();
	}
}
