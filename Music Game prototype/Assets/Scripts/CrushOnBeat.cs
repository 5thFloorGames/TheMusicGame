using UnityEngine;
using System.Collections;

public class CrushOnBeat : MonoBehaviour, Beater {

	public bool up = false;
	public int startCount = 0;
	private int count = 0;
	public int everyXBeat = 2;
	private AudioSource snare;

	void Awake() {
		snare = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
		count = startCount;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Beat (){
		count++;
		if(count >= everyXBeat){
			if (up) {
				transform.position = new Vector3(transform.position.x,transform.position.y - 4,transform.position.z);
				up = false;
				snare.Play();
			} else {
				transform.position = new Vector3(transform.position.x,transform.position.y + 4,transform.position.z);
				up = true;
			}
			count = 0;
		}
	}
}
