using UnityEngine;
using System.Collections;

public class MoveLeft : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = (transform.position + Vector3.left * 0.1f);
	}
}
