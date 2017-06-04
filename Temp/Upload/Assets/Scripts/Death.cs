using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Death : MonoBehaviour {

    public GameManager gameManager;
    public RigidbodyFirstPersonController playerController;
    public Timer timer;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Restart the level
            timer.Stop();
            playerController.Immobilize();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.death);
            StartCoroutine("deathTimer");
        }
    }

    IEnumerator deathTimer() {
        yield return new WaitForSeconds(1);
        gameManager.Restart();
    }
}
