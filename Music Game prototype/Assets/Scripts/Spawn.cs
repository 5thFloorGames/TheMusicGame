using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;

public class Spawn : MonoBehaviour, Beater {

	public GameObject toSpawn;
	private AudioSource audioSource;
	private PlatformerCharacter2D player;
	private AudioClip[] shootingSounds;
	private Queue<Transform> shotQueue;
	private BeatMatcher beat;

	void Awake() {
		audioSource = GetComponent<AudioSource> ();
		shootingSounds = Resources.LoadAll<AudioClip>("Audio/Projectiles/Short");
		player = FindObjectOfType<PlatformerCharacter2D> ();
		shotQueue = new Queue<Transform> ();
		beat = FindObjectOfType<BeatMatcher> ();
	}

	// Use this for initialization
	void Start () {
		beat.registerBeat (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Beat(){
		if (shotQueue.Count > 0) {
			Transform platformTransform = shotQueue.Dequeue ();
			Instantiate (toSpawn, new Vector3 (platformTransform.position.x, platformTransform.position.y, platformTransform.position.z + 440), Quaternion.identity);
			audioSource.PlayOneShot (shootingSounds [(int)player.activeNote], 0.4f);
		}
	}

	public void SpawnProjectile(Transform platformTransform){
		shotQueue.Enqueue (platformTransform);
	}
}
