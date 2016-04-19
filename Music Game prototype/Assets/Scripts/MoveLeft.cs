using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class MoveLeft : MonoBehaviour {

	private float startingX;
	private bool held = false;

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
				if(held){
					GameObject.FindGameObjectWithTag("Player").SendMessage("Unfreeze");
				}
				Destroy(gameObject);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			if(!col.GetComponent<PlatformerCharacter2D>().frozen){
				held = true;
				StartCoroutine(HoldPlayer(col.gameObject));
			}
		}
	}

	IEnumerator HoldPlayer(GameObject player){
		player.SendMessage("Freeze");
		while (true) {
			yield return new WaitForEndOfFrame();
			player.transform.position = transform.position;
		}
	}
}
