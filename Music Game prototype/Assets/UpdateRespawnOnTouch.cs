using UnityEngine;
using System.Collections;

public class UpdateRespawnOnTouch : MonoBehaviour {

	private RespawnManager manager;

	void Awake(){
		manager = FindObjectOfType<RespawnManager> ();
	}
		
	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			manager.UpdateRespawn(transform);
		}
	}
}
