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
            gameManager.SaveTime();
			winText.gameObject.SetActive(true);
			winText.text = "Test complet.\n Congratulations for not dying. \n Your test sequence was achieved in:";
			winText.text += System.Math.Round((decimal)timer.GetTimer(), 2).ToString();

			gameManager.NoControl();
            if (ApplicationModel.levelSelectionMode)
            {
                StartCoroutine("GoToMainMenu");
            }
            else
            {
                StartCoroutine("GoToLevelEndTimer");
            }
        }
	}

	IEnumerator RestartTimer() {
        yield return new WaitForSeconds(4);
		winText.gameObject.SetActive(false);
        gameManager.Restart();
    }
	IEnumerator GoToLevelEndTimer(){
		yield return new WaitForSeconds(1);
		gameManager.isGoingToLevelEnd = true;
	}
    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(3);
        gameManager.GoToMainMenu();
    }

}
