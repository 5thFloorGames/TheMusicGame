using UnityEngine;
using System.Collections;

public class PauseLauncherOnTouch : MonoBehaviour {

	private LauncherLogic launcher;

	// Use this for initialization
	void Start () {
		launcher = FindObjectOfType<LauncherLogic> ();
	}
	
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			launcher.UnpauseLauncher();
		}
	}
}