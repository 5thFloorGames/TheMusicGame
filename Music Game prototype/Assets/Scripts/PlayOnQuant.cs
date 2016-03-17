using UnityEngine;
using System.Collections;

public class PlayOnQuant : MonoBehaviour, Quanter {

	private AudioSource source;
	private BeatMatcher beat;
	private AudioClip[] clips;

	void Awake() {
		source = GetComponent<AudioSource> ();
		beat = FindObjectOfType<BeatMatcher>();
		clips = Resources.LoadAll<AudioClip>("Audio/Melodies");
	}

	// Use this for initialization
	void Start () {
		beat.registerQuant (this);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Act(){
		source.PlayOneShot(clips [Random.Range (0, clips.Length)]);
	}
}
