using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour, Quanter {

	BeatMatcher beat;
	public GameObject destructionEffect;

	void Awake(){
		beat = FindObjectOfType<BeatMatcher> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DestroyOnQuant(){
		beat.registerQuantOneOff (this);
	}

	public void Act(){
		Instantiate (destructionEffect);
		gameObject.SetActive (false);
	}
}
