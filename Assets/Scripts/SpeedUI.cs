using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class SpeedUI : UnityEngine.MonoBehaviour {

	public GameObject player;
	private Rigidbody playerRigidbody;
	private Text speedText;

	// Use this for initialization
	void Start () {
		speedText = GetComponent<Text>();
		playerRigidbody = player.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		//speedText.text = "Vitesse : " + System.Math.Round((decimal)playerRigidbody.velocity.magnitude, 2).ToString();
		Vector3 forwardSpeed = Vector3.Project(playerRigidbody.velocity, player.transform.forward);
		speedText.text = "Speed : " + ((int)forwardSpeed.magnitude).ToString();
	}
}
