using UnityEngine;
using System.Collections;

public class BoostOnTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			col.gameObject.SendMessage("SpeedBoost");
		}
	}
}
