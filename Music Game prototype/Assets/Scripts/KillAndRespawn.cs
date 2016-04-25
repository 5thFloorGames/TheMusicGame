using UnityEngine;
using System.Collections;

public class KillAndRespawn : MonoBehaviour {

	public Transform respawn;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		Application.LoadLevel("BackgroundAssets");
		//col.transform.position = respawn.position;
	}
}
