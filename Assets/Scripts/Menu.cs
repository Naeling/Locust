using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Menu : MonoBehaviour {

	public Canvas mainMenu;
	public Canvas levelSelectionMenu;

	void Start() {
		mainMenu.enabled = true;
		levelSelectionMenu.enabled = false;
	}


	/****
		Start of Main Menu Section
	*****/

	// TODO change the instruction to launch the first cinematic
	public void StartGame() {
		StartLevel(1);
	}

 	public void Quit() {
		Application.Quit();
	}

	public void GoToLevelSelection(){
		Debug.Log("OK");
		mainMenu.enabled = false;
		levelSelectionMenu.enabled = true;
	}

	/****
		End of Main Menu Section
	*****/


	/****
		Start of Level Selection Section
	*****/

	public void StartLevel1() {
		SceneManager.LoadScene("Level 1");
	}
	public void StartLevel2() {
		SceneManager.LoadScene("Level 2");
	}
	public void StartLevel(int levelNumber) {
		SceneManager.LoadScene("Level " + levelNumber.ToString());
	}
	public void LevelNotAvailable() {
		Debug.Log("The requested level is not available yet");
	}
	public void GoToMainMenu() {
		mainMenu.enabled = true;
		levelSelectionMenu.enabled = false;
	}
	/****
		End of Level Selection Section
	****/

}
