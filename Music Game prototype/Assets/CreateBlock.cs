using UnityEngine;
using System.Collections;

public class CreateBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Instantiate (Resources.LoadAll<GameObject>("DashBlocks")[Random.Range(0,6)], transform.position, Quaternion.identity);
		GetComponent<MeshRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
