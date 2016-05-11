using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (DestroyInXSeconds (2f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator DestroyInXSeconds(float seconds){
		yield return new WaitForSeconds (seconds);
		Destroy (gameObject);
	}
}
