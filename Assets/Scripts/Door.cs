using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Door : MonoBehaviour
{

    Animator animator;
    bool doorOpen;

  

    private void Start()
    {
        doorOpen = false;
        animator = GetComponent<Animator>();
    }



    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            doorOpen = true;
            Doors("Open");
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            doorOpen = false;
            Doors("Close");
        }
    }

    void Doors(string direction)
    {
        animator.SetTrigger(direction);
    }
}
