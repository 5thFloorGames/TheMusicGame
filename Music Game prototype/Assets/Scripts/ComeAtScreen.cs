using UnityEngine;
using System.Collections;

public class ComeAtScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.z > 0) {
			transform.position = (new Vector3 (transform.position.x, transform.position.y, transform.position.z - 4));
		}
		if (transform.position.z == 0) {
			Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, 1f);
			foreach(Collider2D col in colliders){
				if(col.gameObject.tag == "Platform"){
					col.gameObject.SetActive(false);
				}
			}
			Destroy(gameObject);
		}
	}
}
