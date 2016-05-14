using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour, Quanter {

	BeatMatcher beat;
	public GameObject destructionEffect;
	Renderer rend;
	public Material cracked;
	public Material destroyedOn;
	public Material destroyedOff;
	private GameObject player;
	
	void Awake(){
		beat = FindObjectOfType<BeatMatcher> ();
		rend = gameObject.GetComponent<Renderer> ();
		player = GameObject.FindGameObjectWithTag("Player");
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
		player.SendMessage ("SpeedBoost");
		Instantiate (destructionEffect);
		rend.material = destroyedOn;
		FindObjectOfType<ScreenShake> ().jiggleCam (0.1f, 0.2f);
		gameObject.GetComponent<ChangeTextureOnBeat> ().updateTextures (destroyedOn, destroyedOff);
		gameObject.GetComponent<Collider2D> ().enabled = false;
	}
}
