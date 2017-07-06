using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

	public Image blackBackground;

	// Use this for initialization
	void Start () {
		blackBackground.CrossFadeAlpha(0f, 5f, false);
		StartCoroutine("turnBlack");
	}

	IEnumerator  turnBlack() {
		yield return new WaitForSeconds(10);
		blackBackground.CrossFadeAlpha(1f, 3f, false);
		StartCoroutine("StartGame");
	}

	IEnumerator StartGame (){
		yield return new WaitForSeconds(6);
		SceneManager.LoadScene("Level0");
	}


}
