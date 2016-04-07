using UnityEngine;
using System.Collections;

public class LauncherLogic : MonoBehaviour, Beater{

	private ShootProjectiles[] launchers;
	public GameObject character;
	public bool enabled;

	// Use this for initialization
	void Start () {
		launchers = GetComponentsInChildren<ShootProjectiles>();
		FindObjectOfType<BeatMatcher> ().registerBeat (this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Beat(){
		if (enabled) {
			transform.position = (character.transform.position + Vector3.right * 9);
			launchers [Random.Range (0, launchers.Length)].Shoot ();
		}
	}
}
