using UnityEngine;
using System.Collections;

public class EndGameOnTouch : MonoBehaviour {

	private bool done = false;

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && !done) {
			col.gameObject.SendMessage("FloatUp");
			done = true;
		}
	}
}
