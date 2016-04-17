using UnityEngine;
using System.Collections;

public class ActivateLauncherOnTouch : MonoBehaviour {

	private LauncherLogic launcher;
	public TriggerType type;

	void Awake(){
		launcher = FindObjectOfType<LauncherLogic> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			if(type == TriggerType.Activate){
				launcher.enabled = true;
			} else if(type == TriggerType.Deactivate){
				launcher.enabled = false;
			}
		}
	}
}
