using UnityEngine;
using System.Collections;

public class CreateBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Instantiate (Resources.LoadAll<GameObject>("DashBlocks")[Random.Range(1,6)], transform.position + Vector3.up * 2.8f, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
