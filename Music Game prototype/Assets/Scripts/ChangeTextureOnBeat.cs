using UnityEngine;
using System.Collections;

public class ChangeTextureOnBeat : MonoBehaviour, Beater {

	Renderer rend;
	public Material on;
	public Material off;
	bool materialOn = false;
	private BeatMatcher beat;

	void Awake(){
		rend = gameObject.GetComponent<Renderer> ();
		beat = FindObjectOfType<BeatMatcher> ();
	}

	// Use this for initialization
	void Start () {
		beat.registerBeat (this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateTextures(Material on, Material off){
		this.on = on;
		this.off = off;
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
