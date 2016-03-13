using UnityEngine;
using System.Collections;

public class ChangeTextureOnBeat : MonoBehaviour, Beater {

	Renderer rend;
	public Material on;
	public Material off;
	bool materialOn = false;
	
	// Use this for initialization
	void Start () {
		rend = gameObject.GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Beat (){
		if (!materialOn) {
			rend.material = on;
			materialOn = true;
		} else {
			rend.material = off;
			materialOn = false;		}
	}
}
