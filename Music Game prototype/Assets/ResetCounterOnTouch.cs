using UnityEngine;
using System.Collections;

public class ResetCounterOnTouch : MonoBehaviour {

	private CountDownOnBeat count;	

	// Use this for initialization
	void Awake () {
		count = FindObjectOfType<CountDownOnBeat> ();
	}

	void OnTriggerEnter2D(Collider2D col){
		count.ResetAndStop ();
	}
	
	void OnTriggerExit2D(Collider2D col){
		count.Continue ();
	}
}
