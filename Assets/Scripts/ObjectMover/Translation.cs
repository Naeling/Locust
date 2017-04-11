using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Translation : ObjectSwitcher {

	private Vector3 translationDirection;
	public float translationMultiplier;
	public float translationSpeedMultiplier;
	// Indicates if the Object is in the state true or false, default true
	private Boolean state;
	// The Object is going to translate between the two positions
	private Vector3 position1;
	private Vector3 position2;


	// On suppose qu'on souhaite ici implémenter une translation d'une valeur fixe dans une direction donnée
	// On a donc deux positions associées à l'object, une pour chaque état


	// Eventuellement possibilité d'interrompre une transition en cours de réalisation
	//
	public void Start(){
		translationDirection = transform.forward;
		state = true;
		// This instruction is perfectly fine
		position1 = transform.position;
		// This instruction should be sign independant thanks to translateDirection taking account for the sign
		position2 = position1 + translationDirection * translationMultiplier;
	}
	public void Update(){
		if (state){
			// Then the object should be translating to position1
			if ( Vector3.Distance(position1, transform.position) > Vector3.kEpsilon * 4000){
				transform.Translate(-translationDirection * Time.deltaTime * translationSpeedMultiplier, Space.World);
			}
		} else {
			// Then the object should be translating to position2
			if ( Vector3.Distance(position2, transform.position) > Vector3.kEpsilon * 4000){
				transform.Translate(translationDirection * Time.deltaTime * translationSpeedMultiplier, Space.World);
			}
		}
	}

	public override void Switch(){
		// Do something
		state = !state;
	}

}
