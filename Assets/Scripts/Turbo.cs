using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Turbo : MonoBehaviour {

	public RigidbodyFirstPersonController playerController;
	private RectTransform turboBar;

	void Start () {
		turboBar = GetComponent<RectTransform>();
	}

	void Update () {
		Vector3 oldScale = turboBar.localScale;
		
		turboBar.localScale =
	}
}
