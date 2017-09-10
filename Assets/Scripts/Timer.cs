using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour {

	private Text timerText;
	private float timer;
	private Boolean isRunning;


 	private void Awake() {
		timerText = GetComponent<Text>();
		Reset();
	}
 	private void Update () {
		if (isRunning){
			timer += Time.deltaTime;
		}
		timerText.text = "Temps : " + System.Math.Round((decimal)timer, 2).ToString();
	}
	public void Go(){
		isRunning = true;
	}
	public void Stop(){
		isRunning = false;
	}
	public void Reset () {
		timer = 0f;
	}
	public float GetTimer() {
		return timer;
	}
}
