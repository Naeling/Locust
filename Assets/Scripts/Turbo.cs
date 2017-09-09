using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Turbo : UnityEngine.MonoBehaviour {

	public RigidbodyFirstPersonController playerController;
	public float UiScaleMultiplier;
	private RectTransform turboBar;


	void Start () {
		turboBar = GetComponent<RectTransform>();
	}

	void Update () {
		float turboPoints = playerController.GetTurboPoints();
		turboBar.localScale = new Vector3(turboPoints * UiScaleMultiplier, turboBar.localScale.y);
	}
}
