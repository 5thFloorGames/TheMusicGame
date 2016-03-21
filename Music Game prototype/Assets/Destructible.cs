using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour, Quanter {

	BeatMatcher beat;
	public GameObject destructionEffect;
	Renderer rend;
	public Material cracked;
	public Material destroyed;
	
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
		rend.material = cracked;
	}

	public void Act(){
		Instantiate (destructionEffect);
		rend.material = destroyed;
		gameObject.GetComponent<Collider2D> ().enabled = false;
	}
}
