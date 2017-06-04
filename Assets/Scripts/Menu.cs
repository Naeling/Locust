using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;




public class Menu : MonoBehaviour {

	// Canvas
	public Canvas mainUI;
	public Canvas levelSelectionUI;
	public Canvas highScoresUI;

	// Text
	public Text level1HighScoresText;
	public Text level2HighScoresText;

	void Start() {
		mainUI.enabled = true;
		levelSelectionUI.enabled = false;
		highScoresUI.enabled = false;

		// Setup the highscores list
		for (int i = 1; i <= 2; i++){
			setupHighScoresForLevel(i);
		}
	}


	/****
		Start of Main Menu Section
	*****/

	// TODO change the instruction to launch the first cinematic
	public void StartGame() {
		StartLevel(1);
	}

	public void GoToLevelSelection(){
		mainUI.enabled = false;
		levelSelectionUI.enabled = true;
	}

	public void GoToHighScores(){
		mainUI.enabled = false;
		highScoresUI.enabled = true;

	}
	public void Quit() {
		Application.Quit();
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
		mainUI.enabled = true;
		levelSelectionUI.enabled = false;
	}
	/****
		End of Level Selection Section
	****/

	/****
		Start of HighScores List Section
	*****/

	public void setupHighScoresForLevel(int i){
		var times = LoadPreviousTimes("Level " + i.ToString());
		var topTwenty = times.OrderBy(time => time.time).Take(20);
		Text timesLabel;
		switch (i){
			case 1:
				timesLabel = level1HighScoresText;
				break;
			case 2:
				timesLabel = level2HighScoresText;
				break;
			default:
				timesLabel = level1HighScoresText;
				break;
		}
		timesLabel.text = "";
		foreach (var time in topTwenty) {
			//speedText.text = "Vitesse : " + System.Math.Round((decimal)playerRigidbody.velocity.magnitude, 2).ToString();
			timesLabel.text += System.Math.Round(time.time, 3) + "\n";
		}
	}

	// Get the times for a specific level
	public List<PlayerTimeEntry> LoadPreviousTimes(String levelName) {
		try {
			var scoresFile = Application.persistentDataPath + "/" + "player1" + levelName + "_times.dat";
			using (var stream = File.Open(scoresFile, FileMode.Open)) {
				var bin = new BinaryFormatter();
				var times = (List<PlayerTimeEntry>)bin.Deserialize(stream);
				return times;
			}
		}
		catch (IOException ex) {
			Debug.LogWarning("Couldn’t load previous times for: " + "player1" + ". Exception: " + ex.Message);
			return new List<PlayerTimeEntry>();
		}
	}

	/****
		End of HighScores List Section
	*****/

}
