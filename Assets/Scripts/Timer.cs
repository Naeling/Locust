using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	private Text timerText;
	private float timer;


 	private void Awake() {
		timerText = GetComponent<Text>();
		Reset();
	}
 	private void Update () {
		timer += Time.deltaTime;
		timerText.text = System.Math.Round((decimal)timer, 2).ToString();
	}
	public void Reset () {
		timer = 0f;
	}
	public float GetTimer() {
		return timer;
	}
}
