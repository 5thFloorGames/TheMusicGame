using UnityEngine;
using System.Collections;

public class MoveLeft : MonoBehaviour {

	private float startingX;

	// Use this for initialization
	void Start () {
		startingX = transform.position.x;
		StartCoroutine (CheckForDestruction ());
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = (transform.position + Vector3.left * 0.1f);
	}

	IEnumerator CheckForDestruction(){
		while (true) {
			yield return new WaitForSeconds(1f);
			if (transform.position.x - startingX < -40){
				Destroy(gameObject);
			}
		}
	}
}
