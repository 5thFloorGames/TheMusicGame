using UnityEngine;
using System.Collections;

public class PlayOnTouch : MonoBehaviour {

	private AudioSource sound;

	void Awake() {
		sound = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		print ("hit");
		sound.volume = 0.5f;
	}

	void OnTriggerExit2D(Collider2D col){
		sound.volume = 0;
	}
}
