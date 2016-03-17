using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class PlayOnTouch : MonoBehaviour {

	public Note note;
	private PlatformerCharacter2D player;

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlatformerCharacter2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D(Collider2D col){
		player.setClip (note);
	}

	void OnTriggerExit2D(Collider2D col){
		//player.setClip (null);
	}
}
