using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Intro : MonoBehaviour {

	private AudioSource audioSource;
	public Image blackBackground;
	public RigidbodyFirstPersonController playerController;
	public Animator doorAnimator;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		audioSource.Play();
	}

	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying){
			doorAnimator.SetBool("introFinished", true);
			playerController.Move();
		}
	}

	public void IntroScreen() {
		blackBackground.CrossFadeAlpha(1f, 1f, false);
		StartCoroutine("IntroScreenCoroutine");
	}

	IEnumerator IntroScreenCoroutine () {
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene("IntroScreen");
	}

	IEnumerator NextLevelTimer() {
        yield return new WaitForSeconds(3);
		SceneManager.LoadScene("Level0");
    }
}
