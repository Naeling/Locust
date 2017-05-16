using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class SpeedUI : UnityEngine.MonoBehaviour {

	public Rigidbody playerRigidbody;
	private Text speedText;

	// Use this for initialization
	void Start () {
		speedText = GetComponent<Text>();
	}

	// Update is called once per frame
	void Update () {
		speedText.text = "Vitesse : " + System.Math.Round((decimal)playerRigidbody.velocity.magnitude, 2).ToString();

	}
}
