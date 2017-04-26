using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject player;
	private Transform playerTransform;
	private Rigidbody playerRigidbody;
	public Transform spawnPoint;
	private MouseLook cameraController;
	private RigidbodyFirstPersonController playerController;
	public Timer timer;


	void Start () {
		playerTransform = player.GetComponent<Transform>();
		playerRigidbody = player.GetComponent<Rigidbody>();
		cameraController = player.GetComponent<RigidbodyFirstPersonController>().mouseLook;
		playerController = player.GetComponent<RigidbodyFirstPersonController>();
	}

	void Update () {
		if (CrossPlatformInputManager.GetButtonDown("Restart"))
		{
			Restart();
		}
	}
	void Restart () {
		Debug.Log("Restart requested");
		cameraController.resetRotations();
		playerRigidbody.velocity = new Vector3();
		playerTransform.position = spawnPoint.position;
		playerTransform.rotation = Quaternion.identity;
		timer.Reset();
	}
}
