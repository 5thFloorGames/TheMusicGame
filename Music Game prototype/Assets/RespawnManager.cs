using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour {

	private Transform activeRespawn;
	public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateRespawn(Transform transform){
		activeRespawn = transform;
	}

	public void RespawnPlayer(){
		player.transform.position = activeRespawn.position;
		player.SendMessage ("StartFreeze");
	}
}
