using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class FinalDialog : MonoBehaviour {

	private AudioSource audioSource;
	public AudioClip finalDialog;
	private Boolean finalDialogStarted;
	public RisingDeath risingDeath;
	public Image blackBackground;
	//public FallingBlock fallingBlock;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		finalDialogStarted = false;
	}

	// Update is called once per frame
	void Update () {
		if (finalDialogStarted && !audioSource.isPlaying){
			blackBackground.CrossFadeAlpha(1f, 3f, false);
			StartCoroutine("Ending");
			//risingDeath.Move();
			//fallingBlock.Fall();

		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player"){
			audioSource.PlayOneShot(finalDialog);
			finalDialogStarted = true;
		}
	}

	IEnumerator Ending(){
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene("EndingScreen");
	}

}
