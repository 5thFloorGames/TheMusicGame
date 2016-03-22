using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

	public GameObject toSpawn;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SpawnProjectile(Transform platformTransform){
		Instantiate (toSpawn, new Vector3 (platformTransform.position.x, platformTransform.position.y, platformTransform.position.z + 440), Quaternion.identity);
	}
}
