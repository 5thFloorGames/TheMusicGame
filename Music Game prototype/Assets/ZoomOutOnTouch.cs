using UnityEngine;
using System.Collections;

public class ZoomOutOnTouch : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			FindObjectOfType<Camera>().SendMessage("ZoomOut");
		}
	}

}
