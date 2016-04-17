using UnityEngine;
using System.Collections;

public class DropCamera : MonoBehaviour {

	public bool enable = true;
	private Camera2DFollow cameraSettings;
	private bool done = false;

	void Awake(){
		cameraSettings = FindObjectOfType<Camera2DFollow> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && !done && enable) {
			cameraSettings.damping = 0.05f;
			cameraSettings.lookAheadFactor = 0f;
			done = true;
		}
		if (col.tag == "Player" && !done && !enable) {
			cameraSettings.damping = 0.5f;
			cameraSettings.lookAheadFactor = 15f;
			done = true;
		}
	}
}
