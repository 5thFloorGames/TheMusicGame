using UnityEngine;
using System.Collections;

public class ShootOnTouch : MonoBehaviour {

	private Spawn spawner;

	// Use this for initialization
	void Awake () {
		spawner = FindObjectOfType<Spawn> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		spawner.SpawnProjectile (transform);
	}
}
