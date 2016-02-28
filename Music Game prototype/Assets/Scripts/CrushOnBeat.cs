using UnityEngine;
using System.Collections;

public class CrushOnBeat : MonoBehaviour, Beater {

	private bool up = false;
	private int count = 0;
	private int everyXBeat = 2;
	private AudioSource snare;

	void Awake() {
		snare = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
		
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
			} else {
				transform.position = new Vector3(transform.position.x,transform.position.y + 4,transform.position.z);
				up = true;
			}
			count = 0;
		}
	}
}
