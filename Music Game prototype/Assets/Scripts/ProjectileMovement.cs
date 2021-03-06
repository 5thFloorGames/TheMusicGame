﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class ProjectileMovement : MonoBehaviour, Quanter {

	private float startingX;
	private bool held = false;
	private Coroutine playerHeld;

	// Use this for initialization
	void Start () {
		startingX = transform.position.x;
		StartCoroutine (CheckForDestruction ());
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = (transform.position + Vector3.left * 3.5f * Time.deltaTime);
	}

	IEnumerator CheckForDestruction(){
		while (true) {
			yield return new WaitForSeconds(1f);
			if (transform.position.x - startingX < -40){
				if(playerHeld == null){
					Destroy(gameObject);
				}
			}
		}
	}

	void Release(){
		if(held){
			StopCoroutine(playerHeld);
			playerHeld = null;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.SendMessage("Unfreeze");
			player.SendMessage("BubbleRelease");
			FindObjectOfType<LauncherLogic>().UnpauseLauncher();
		}
	}

	public void Act(){
		Release ();
	}
	
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && !held) {
			if(!col.GetComponent<PlatformerCharacter2D>().frozen){
				held = true;
				playerHeld = StartCoroutine(HoldPlayer(col.gameObject));
			}
		}
	}

	IEnumerator HoldPlayer(GameObject player){
		player.SendMessage("Freeze");
		player.SendMessage("BubbleCatch");
		FindObjectOfType<ScreenShake> ().jiggleCam (0.1f, 2f);
		FindObjectOfType<LauncherLogic>().PauseLauncher();
		FindObjectOfType<BeatMatcher> ().TriggerInXBeats (this, 4);
		while (true) {
			yield return new WaitForEndOfFrame();
			player.transform.position = transform.position;
		}
	}
}
