using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance = null;
	private AudioSource audioSource;

	public AudioClip death;
	public AudioClip intro;
	public AudioClip conclusion;

	// Use this for initialization
	void Start () {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
		audioSource = GetComponent<AudioSource>();

	}

	public void PlayOneShot(AudioClip clip){
		audioSource.PlayOneShot(clip);
	}

	public AudioSource GetAudioSource(){
		return audioSource;
	}

}
