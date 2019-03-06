using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrogPlatforms;

/*
*	Class: Platform Decorator
*	Implements DecorOptions to dynamically toggle decoration variations
*	At runtime.
 */

public class platformDecorator : MonoBehaviour {

	public DecoVariation[] firstOptions;
	DecorOptions instanceOptions = new DecorOptions(){};

	// Use this for initialization
	void Start () {
		DecorOptions startOptions = new DecorOptions(firstOptions){};
		instanceOptions = startOptions;
		instanceOptions.selectVariation();
	}

	//This function gets called with a "sendMessage" method
	//Basically, as long as there's more than one possible decoration set,
	//de-activate all current sets on the platform and pick a new one by calling the built-in method.
	//a SendMessage inside the "PoolableObject.cs" script
	public void resetInstanceDecor() {
		if(this.instanceOptions.reportLength() > 1) {
			instanceOptions.resetVariation();
		}
	}
	
}
