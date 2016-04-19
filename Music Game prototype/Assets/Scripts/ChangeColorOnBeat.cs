using UnityEngine;
using System.Collections;

public class ChangeColorOnBeat : MonoBehaviour, Beater {

	Renderer rend;

	// Use this for initialization
	void Start () {
		rend = gameObject.GetComponent<Renderer> ();
	}

	public void Beat (){
		if (rend.material.color == Color.green) {
			rend.material.color = Color.black;
		} else {
			rend.material.color = Color.green;
		}
	}
}
