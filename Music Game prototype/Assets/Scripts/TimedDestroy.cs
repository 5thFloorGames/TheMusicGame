using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour {

	public float secondsToLive = 2f;

	// Use this for initialization
	void Start () {
		StartCoroutine (DestroyInXSeconds (secondsToLive));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator DestroyInXSeconds(float seconds){
		yield return new WaitForSeconds (seconds);
		Destroy (gameObject);
	}
}
