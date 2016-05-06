using UnityEngine;
using System.Collections;

public class BoostOnTouch : MonoBehaviour {

	private bool activated = false;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (!activated) {
			if (col.tag == "Player") {
				col.gameObject.SendMessage("SpeedBoost");
				activated = true;
		 	}
		}
	}
}
