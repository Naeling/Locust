using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour {

	public GameObject player;
	private Transform playerTransform;
	private Rigidbody playerRigidbody;
	public Transform spawnPoint;
	private MouseLook cameraController;
	private RigidbodyFirstPersonController playerController;
	public Timer timer;
	public String playerName;


	void Start () {
		playerTransform = player.GetComponent<Transform>();
		playerRigidbody = player.GetComponent<Rigidbody>();
		cameraController = player.GetComponent<RigidbodyFirstPersonController>().mouseLook;
		playerController = player.GetComponent<RigidbodyFirstPersonController>();
		DisplayPreviousTimes();
	}

	void Update () {
		if (CrossPlatformInputManager.GetButtonDown("Restart"))
		{
			Restart();
		}
	}
	public void Restart () {
		Debug.Log("Restart requested");
		cameraController.resetRotations();
		playerRigidbody.velocity = new Vector3();
		playerTransform.position = spawnPoint.position;
		playerTransform.rotation = Quaternion.identity;
		timer.Reset();
	}
	public List<PlayerTimeEntry> LoadPreviousTimes() {
		try {
			// Get the file corresponding to the current player Name
			var scoresFile = Application.persistentDataPath + "/" + playerName + "_times.dat";
			// Open the file
			using (var stream = File.Open(scoresFile, FileMode.Open)) {
				// Binary Formatter
				var bin = new BinaryFormatter();
				// Deserialize the file contents into the List of PlayerTimeEntry
				var times = (List<PlayerTimeEntry>)bin.Deserialize(stream);
				return times;
			}
		}
		catch (IOException ex) {
			Debug.LogWarning("Couldn’t load previous times for: " + playerName + ". Exception: " + ex.Message);
			return new List<PlayerTimeEntry>();
		}
	}
	// use a decimal
 	public void SaveTime() {
		var time = timer.GetTimer();
		var times = LoadPreviousTimes();
		var newTime = new PlayerTimeEntry();
		newTime.entryDate = DateTime.Now;
		newTime.time = (Decimal)time;
		var bFormatter = new BinaryFormatter();
		var filePath = Application.persistentDataPath + "/" + playerName + "_times.dat";
		using (var file = File.Open(filePath, FileMode.Create)) {
			times.Add(newTime);
			bFormatter.Serialize(file, times);
		}
	}
	public void DisplayPreviousTimes() {
		var times = LoadPreviousTimes();
		var topThree = times.OrderBy(time => time.time).Take(3);
		var timesLabel = GameObject.Find("PreviousTimes").GetComponent<Text>();
		timesLabel.text = "BEST TIMES \n";
		foreach (var time in topThree) {
			timesLabel.text += time.entryDate.ToShortDateString() + ": " + time.time + "\n";
		}
	}
}
