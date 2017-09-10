using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class Intro : MonoBehaviour {

	private AudioSource audioSource;
	public Image blackBackground;
	public RigidbodyFirstPersonController playerController;
	public Animator doorAnimator;
    private bool wantToSkipDialogue;
    public GameObject skipText;

    void Start () {
		audioSource = GetComponent<AudioSource>();
		audioSource.Play();
	}

	void Update () {

		if (!audioSource.isPlaying){
			doorAnimator.SetBool("introFinished", true);
			playerController.Move();
		} else
        {
            if (wantToSkipDialogue)
            {
                if (CrossPlatformInputManager.GetButtonDown("Cancel"))
                {
                    audioSource.Stop();
                    skipText.SetActive(false);
                }
            }
            else
            {
                if (CrossPlatformInputManager.GetButtonDown("Jump"))
                {
                    wantToSkipDialogue = true;
                    skipText.SetActive(true);
                    StartCoroutine("HideSkipText");
                }
            }
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

    IEnumerator HideSkipText()
    {
        yield return new WaitForSeconds(3);
        skipText.SetActive(false);
        wantToSkipDialogue = false;
    }
}
