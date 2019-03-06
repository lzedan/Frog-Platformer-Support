using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InfiniteRunnerEngine;
using FrogPlatforms;

/***********************************************
	entityConnectedCoin
	Implements EntityObject specific to a coin instance
	And adds additional behaviour for coin interactions
***********************************************/


public class entityConnectedCoin : MonoBehaviour {
	public System.String entityType = "coin";
	public float pointsToAdd = 1;
	public int pointsCollection = 1;

	public AudioClip coinCollected;

	EntityObject subCoin = new EntityObject(){};

	// Use this for initialization
	void Start () {
		subCoin = new EntityObject(entityType, pointsToAdd, pointsCollection, this.gameObject){};
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.tag == "Player") {
			SoundManager.Instance.PlaySound(coinCollected,transform.position);
			subCoin.addPoints();
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
		if(col.gameObject.tag == "Recycler"){
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
		}
	}
}
