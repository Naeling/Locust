using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

	public GameManager gameManager;
	public Text winText;
	public Timer timer;


	void OnTriggerEnter (Collider other){
		//TODO launch an automatic movement towards the last room, and the last dialog here
		if (other.CompareTag("Player")){
			timer.Stop();
			// Save the time
            gameManager.SaveTime();
			// Display the win text
			winText.gameObject.SetActive(true);
			winText.text = "Test complet.\n Congratulations for not dying. \n Your test sequence was achieved in:";
			winText.text += System.Math.Round((decimal)timer.GetTimer(), 2).ToString();

			gameManager.NoControl();
			StartCoroutine("goToLevelEndTimer");
			// Restart
			//StartCoroutine("restartTimer");
		}
	}


	IEnumerator restartTimer() {
        yield return new WaitForSeconds(4);
		winText.gameObject.SetActive(false);
        gameManager.Restart();
    }
	IEnumerator goToLevelEndTimer(){
		yield return new WaitForSeconds(1);
		gameManager.isGoingToLevelEnd = true;
	}
}
