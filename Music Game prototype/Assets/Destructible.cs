using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour, Quanter {

	BeatMatcher beat;
	public GameObject destructionEffect;
	Renderer rend;
	public Material crackedOn;
	public Material crackedOff;
	public Material destroyedOn;
	public Material destroyedOff;
	
	void Awake(){
		beat = FindObjectOfType<BeatMatcher> ();
		rend = gameObject.GetComponent<Renderer> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DestroyOnQuant(){
		beat.registerQuantOneOff (this);
		rend.material = crackedOn;
		gameObject.GetComponent<ChangeTextureOnBeat> ().updateTextures (crackedOn, crackedOff);
	}

	public void Act(){
		Instantiate (destructionEffect);
		rend.material = destroyedOn;
		gameObject.GetComponent<ChangeTextureOnBeat> ().updateTextures (destroyedOn, destroyedOff);
		gameObject.GetComponent<Collider2D> ().enabled = false;
	}
}
