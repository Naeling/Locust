using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

	public GameManager gameManager;
	public Text winText;
	public Timer timer;

	void OnTriggerEnter (Collider other){
		if (other.CompareTag("Player")){
			timer.Stop();
			// Save the time
            gameManager.SaveTime();
			// Display the win text
			winText.gameObject.SetActive(true);
			winText.text += System.Math.Round((decimal)timer.GetTimer(), 2).ToString();
			// Restart
			StartCoroutine("restartTimer");
		}
	}

	IEnumerator restartTimer() {
        yield return new WaitForSeconds(4);
		winText.gameObject.SetActive(false);
        gameManager.Restart();
    }
}
