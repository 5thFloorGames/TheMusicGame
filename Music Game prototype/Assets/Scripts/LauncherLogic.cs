using UnityEngine;
using System.Collections;

public class LauncherLogic : MonoBehaviour, Beater{

	private ShootProjectiles[] launchers;
	public GameObject character;
	public bool enabled;
	public bool paused = false;

	// Use this for initialization
	void Start () {
		launchers = GetComponentsInChildren<ShootProjectiles>();
		FindObjectOfType<BeatMatcher> ().registerBeat (this);
	}

	public void Beat(){
		if (enabled && !paused) {
			transform.position = (character.transform.position + Vector3.right * 9);
			launchers [Random.Range (0, launchers.Length)].Shoot ();
		}
	}

	public void StartLauncher(){
		enabled = true;
	}

	public void StopLauncher(){
		enabled = false;
	}

	public void PauseLauncher(){
		paused = true;
	}

	public void UnpauseLauncher(){
		paused = false;
	}
}
