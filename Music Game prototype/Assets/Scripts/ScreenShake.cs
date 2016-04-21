using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {
	
	float jiggleAmt=0.0f; // how much to shake
	Vector3 startingPosition;
	
	// Use this for initialization
	void Start () {
		startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(jiggleAmt>0) {
			float quakeAmt = Random.value*jiggleAmt*2 - jiggleAmt;
			Vector3 pp = transform.position;
			pp.y+= quakeAmt; // can also add to x and/or z
			transform.position = pp;
		}
	}
	
	// Others call this to cause an earthquake:
	public void jiggleCam(float amt, float duration) {
		jiggleAmt = amt;
		StartCoroutine(jiggleCam2(duration));
	}
	
	
	IEnumerator jiggleCam2(float duration) {
		yield return new WaitForSeconds(duration);
		jiggleAmt=0;
		//transform.position = startingPosition;
	}
}