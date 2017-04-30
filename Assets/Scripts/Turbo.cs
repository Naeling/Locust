using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Turbo : MonoBehaviour {

	public RigidbodyFirstPersonController playerController;
	public float UiScaleMultiplier;
	private RectTransform turboBar;


	void Start () {
		turboBar = GetComponent<RectTransform>();
	}

	void Update () {
		// Vector3 oldScale = turboBar.localScale;
		// Need to get the current Turbo points from the controller Object
		float turboPoints = playerController.turboPoints;
		turboBar.localScale = new Vector3(turboPoints * UiScaleMultiplier, turboBar.localScale.y);
	}
}
