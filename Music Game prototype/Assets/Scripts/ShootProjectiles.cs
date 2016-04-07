using UnityEngine;
using System.Collections;

public class ShootProjectiles : MonoBehaviour {

	public GameObject bullet;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Shoot(){
		GameObject tempBullet = (GameObject)Instantiate(bullet, (transform.position + Vector3.right/2), Quaternion.identity);
	}
}
