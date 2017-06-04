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
using UnityEngine.SceneManagement;

public class GameManager : UnityEngine.MonoBehaviour {

	public GameObject player;
	private Transform playerTransform;
	private Rigidbody playerRigidbody;
	public Transform spawnPoint;
	public Transform checkPoint;
	private MouseLook cameraController;
	private RigidbodyFirstPersonController playerController;
	public Timer timer;
	// TODO add a management of several player, need to add a textField in the main Menu
	//public String playerName;
    private Boolean isPause;
    public Canvas gameUI;
    public Canvas pauseUI;
    private Animator UiAnimator;
	public Boolean checkpointReached;
	private String sceneName;


	void Start () {
		playerTransform = player.GetComponent<Transform>();
		playerRigidbody = player.GetComponent<Rigidbody>();
		cameraController = player.GetComponent<RigidbodyFirstPersonController>().mouseLook;
		playerController = player.GetComponent<RigidbodyFirstPersonController>();
        UiAnimator = pauseUI.GetComponent<Animator>();
		checkpointReached = false;
		sceneName = SceneManager.GetActiveScene().name;
		DisplayPreviousTimes();
	}

	void Update () {
		if (CrossPlatformInputManager.GetButtonDown("Restart"))
		{
			Restart();
		}
        if (CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            isPause = !isPause;
            if (isPause)
            {
                // Play animation
                UiAnimator.SetBool("isPaused", true);
                UiAnimator.SetBool("isDepaused", false);

            } else
            {
                // Play animation backward
                UiAnimator.SetBool("isDepaused", true);
                UiAnimator.SetBool("isPaused", false);
            }
        }

        if (isPause)
        {
            Time.timeScale = 0;
            if (gameUI.enabled)
            {
                gameUI.enabled = false;
            }
            if (!pauseUI.enabled)
            {
                pauseUI.enabled = true;
            }
        }
        else
        {
            Time.timeScale = 1;
            if (!gameUI.enabled)
            {
                gameUI.enabled = true;
            }
            if (pauseUI.enabled)
            {
                pauseUI.enabled = false;
            }
        }
    }

	public void Restart () {
		playerController.immobilize = false;
		playerRigidbody.useGravity = true;
		cameraController.resetRotations();
		playerController.turboPoints = 0f;
		playerRigidbody.velocity = new Vector3();
		if (!checkpointReached){
			playerTransform.position = spawnPoint.position;
		} else {
			playerTransform.position = checkPoint.position;
		}
		playerTransform.rotation = Quaternion.identity;
		var qTo = Quaternion.AngleAxis(180f, Vector3.up);
		playerTransform.rotation = Quaternion.identity * qTo;
		timer.Reset();
		timer.Stop();
	}
	public List<PlayerTimeEntry> LoadPreviousTimes() {
		try {
			var scoresFile = Application.persistentDataPath + "/" + "player1" + sceneName + "_times.dat";
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

	// use a decimal
 	public void SaveTime() {
		var time = timer.GetTimer();
		var times = LoadPreviousTimes();
		var newTime = new PlayerTimeEntry();
		newTime.entryDate = DateTime.Now;
		newTime.time = (Decimal)time;
		var bFormatter = new BinaryFormatter();
        Debug.Log(Application.persistentDataPath);
		var filePath = Application.persistentDataPath + "/" + "player1" + sceneName + "_times.dat";
        Debug.Log("Timer time: " + time);
        Debug.Log("Time registered: " + newTime.time);
		using (var file = File.Open(filePath, FileMode.Create)) {
			times.Add(newTime);
			bFormatter.Serialize(file, times);
		}
	}
	public void DisplayPreviousTimes() {
		var times = LoadPreviousTimes();
		var topThree = times.OrderBy(time => time.time).Take(20);
		var timesLabel = GameObject.Find("HighScoresList").GetComponent<Text>();
		timesLabel.text = "";
		foreach (var time in topThree) {
			timesLabel.text += time.time + "\n";
		}
	}
}
