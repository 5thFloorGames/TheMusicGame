using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class PlayOnTouch : MonoBehaviour {

	public Note note;
	private CharacterMusicSystem player;

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<CharacterMusicSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			player.setClip (note);
		}
	}

	void OnTriggerExit2D(Collider2D col){
		//player.setClip (null);
	}

	public void SetNote(Note note){
		this.note = note;
	}
}
