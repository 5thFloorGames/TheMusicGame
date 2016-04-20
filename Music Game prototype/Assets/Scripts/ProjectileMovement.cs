using UnityEngine;
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
		transform.position = (transform.position + Vector3.left * 0.09f);
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
			GameObject.FindGameObjectWithTag("Player").SendMessage("Unfreeze");
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
		FindObjectOfType<LauncherLogic>().PauseLauncher();
		FindObjectOfType<BeatMatcher> ().TriggerInXBars (this, 1);
		while (true) {
			yield return new WaitForEndOfFrame();
			player.transform.position = transform.position;
		}
	}
}
