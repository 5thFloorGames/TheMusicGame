using UnityEngine;
using System.Collections;

public class PlayAndDie : MonoBehaviour {

	AudioSource audioSource;

	void Awake(){
		audioSource = gameObject.GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying) {
			Destroy(gameObject);
		}
	}
}
