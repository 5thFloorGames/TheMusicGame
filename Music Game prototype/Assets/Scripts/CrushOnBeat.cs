using UnityEngine;
using System.Collections;

public class CrushOnBeat : MonoBehaviour, Quanter {

	public bool up = false;
	public int startCount = 0;
	private int count = 0;
	public int everyXQuant = 4;
	private AudioSource snare;
	private BeatMatcher beat;

	void Awake() {
		snare = GetComponent<AudioSource> ();
		beat = FindObjectOfType<BeatMatcher> ();
	}

	// Use this for initialization
	void Start () {
		count = startCount;
		beat.registerQuant (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Act (){
		count++;
		if(count >= everyXQuant){
			if (up) {
				transform.position = new Vector3(transform.position.x,transform.position.y - 3,transform.position.z);
				up = false;
				snare.Play();
			} else {
				transform.position = new Vector3(transform.position.x,transform.position.y + 3,transform.position.z);
				up = true;
			}
			count = 0;
		}
	}
}
