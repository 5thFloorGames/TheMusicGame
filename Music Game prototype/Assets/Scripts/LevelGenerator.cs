﻿using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	private GameObject platform;
	private GameObject lastCreated;
	private int lastLength = 1;
	private Note lastNote;
	private ComposingLogic composingLogic;
	private int levelCounter = 0;

	void Awake(){
		platform = (GameObject)Resources.Load ("Platforms/MusicPlatform");
		composingLogic = GetComponent<ComposingLogic> ();
	}

	// Use this for initialization
	void Start () {
		CreatePlatforms (Note.i, Random.Range (1, 5), transform);
		int tunnel = Random.Range (25, 100);
		int platformLength;
		for (int i = 0; i < 100; i++) {
			if(i != tunnel){
				if(lastLength > 1){
					platformLength = Random.Range (1, 5);
				} else {
					platformLength = Random.Range (2, 5);
				}
				CreatePlatforms(composingLogic.nextNote(lastNote, levelCounter),platformLength,lastCreated.transform);
				levelCounter++;
				if (levelCounter > 3) {
					levelCounter = 0;
				}
			} else {
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Generate a hole with random vertical blocks
	// 3 layered thing that activates the projectiles and then shuts them off at the end

	void CreatePlatforms(Note platformNote, int platformLength, Transform lastPlatform){
		GameObject holder = new GameObject ("Platform " + platformNote);
		GameObject parent = gameObject;
		holder.transform.parent = parent.transform;
		
		bool up = Random.Range (0, 2) == 0;

		int dashBlock = Random.Range (0, 5);
		int shotBlock = Random.Range (0, 1);

		holder.transform.position = lastPlatform.position + (lastLength + platformLength - 1) * (Vector3.right * 5f) + Vector3.right;

		if (up) {
			holder.transform.position += Vector3.up * 4;
		} else {
			holder.transform.position += Vector3.up * -4;
		}

		if (dashBlock == 0) {
			holder.AddComponent<CreateBlock>();
		}
		if (shotBlock == 0) {
			// actually add this to a block with a trigger
			// or add 
			holder.AddComponent<ShootOnTouch>();
		}

		bool paired = platformLength % 2 == 0;
		
		for (float i = -Mathf.Floor(platformLength / 2); i <= Mathf.Floor(platformLength / 2); i++) {
			if(!paired || i != Mathf.Floor(platformLength / 2)){
				GameObject newPlatform = (GameObject)Instantiate (platform);
				newPlatform.GetComponent<PlayOnTouch>().SetNote(platformNote);
				// create a common 2D collider of right size and offset it in y by 1.4 and make it a trigger
				if(paired){
					newPlatform.transform.position = holder.transform.position + new Vector3 ((i * 10) + 5, 0, 0);
				} else {
					newPlatform.transform.position = holder.transform.position + new Vector3 (i * 10, 0, 0);
				}
				newPlatform.transform.parent = holder.transform;
				lastCreated = holder;
				lastNote = platformNote;
				lastLength = platformLength;
			}
		}
	}
}
