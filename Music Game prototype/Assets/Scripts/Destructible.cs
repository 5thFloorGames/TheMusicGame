using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour, Quanter {

	BeatMatcher beat;
	public GameObject destructionEffect;
	Renderer rend;
	public Material cracked;
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
		beat.registerBeatOneOff (this);
		rend.material = cracked;
		gameObject.GetComponent<ChangeTextureOnBeat> ().updateTextures (cracked, cracked);
	}

	public void Act(){
		Instantiate (destructionEffect);
		rend.material = destroyedOn;
		gameObject.GetComponent<ChangeTextureOnBeat> ().updateTextures (destroyedOn, destroyedOff);
		gameObject.GetComponent<Collider2D> ().enabled = false;
	}
}
