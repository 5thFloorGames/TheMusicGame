using UnityEngine;
using System.Collections;

public class ActivateLauncherOnTouch : MonoBehaviour {

	private LauncherLogic launcher;
	public TriggerType type;
	public bool done = false;

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
		if (col.tag == "Player" && !done) {
			if(type == TriggerType.Activate){
				GameObject.FindGameObjectWithTag("Player").SendMessage("LevelUp");
				launcher.enabled = true;
				launcher.UnpauseLauncher();
			} else if(type == TriggerType.Deactivate){
				launcher.enabled = false;
			}
			done = true;
		}
	}
}
