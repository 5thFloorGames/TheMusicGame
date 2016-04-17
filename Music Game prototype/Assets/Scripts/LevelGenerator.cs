using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	private GameObject platform;
	private GameObject lastCreated;
	private int lastCreatedLength = 1;
	private Note lastNote;
	private ComposingLogic composingLogic;
	private int levelCounter = 0;
	private GameObject launcherTrigger;
	private GameObject theDrop;

	void Awake(){
		platform = (GameObject)Resources.Load ("Platforms/MusicPlatform");
		launcherTrigger = Resources.Load<GameObject>("LauncherTrigger");
		theDrop = Resources.Load<GameObject>("Drop");
		composingLogic = GetComponent<ComposingLogic> ();
	}

	// Use this for initialization
	void Start () {
		CreatePlatforms (Note.i, Random.Range (1, 5), transform);
		int tunnel = Random.Range (25, 100);
		int drop = Random.Range (50, 100);
		drop = 3;
		int platformLength;
		for (int i = 0; i < 100; i++) {
			if(i == tunnel){
				Transform start = lastCreated.transform;
				int lastLength = lastCreatedLength;
				Instantiate(launcherTrigger,lastCreated.transform.position, Quaternion.identity);
				CreatePlatforms(composingLogic.randomHighNote(),25,start, lastLength, 4);
				CreatePlatforms(Note.i,25,start, lastLength, 0);
				CreatePlatforms(composingLogic.randomLowNote(),25,start, lastLength, -4);
				GameObject shutdownTrigger = (GameObject)Instantiate(launcherTrigger,lastCreated.transform.position + Vector3.right * 125f, Quaternion.identity);
				shutdownTrigger.GetComponent<ActivateLauncherOnTouch>().type = TriggerType.Deactivate;
			} else if(i == drop){
				GameObject dropObject = (GameObject)Instantiate(theDrop, lastCreated.transform.position + (lastCreatedLength + 1.5f - 1) * (Vector3.right * 5f) + Vector3.right * 3.15f + Vector3.up * 0.75f, Quaternion.identity);
				lastCreated = dropObject.transform.FindChild("Landing").gameObject;
				lastCreatedLength = 2;
			}
				else {
				if(lastCreatedLength > 1){
					platformLength = Random.Range (1, 5);
				} else {
					platformLength = Random.Range (2, 5);
				}
				CreatePlatforms(composingLogic.nextNote(lastNote, levelCounter),platformLength,lastCreated.transform);
				levelCounter++;
				if (levelCounter > 3) {
					levelCounter = 0;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreatePlatforms(Note platformNote, int platformLength, Transform lastPlatform, int lastLength, int heightOffset){
		GameObject holder = new GameObject ("Platform " + platformNote);
		GameObject parent = gameObject;
		holder.transform.parent = parent.transform;
		
		bool up = Random.Range (0, 2) == 0;
		
		int dashBlock = Random.Range (0, 5);
		int shotBlock = Random.Range (0, 1);
		
		holder.transform.position = lastPlatform.position + (lastLength + platformLength - 1) * (Vector3.right * 5f) + Vector3.right;
		
		holder.transform.position += Vector3.up * heightOffset;
		
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
				lastCreatedLength = platformLength;
			}
		}

	}

	// Generate a hole with random vertical blocks
	// 3 layered thing that activates the projectiles and then shuts them off at the end

	void CreatePlatforms(Note platformNote, int platformLength, Transform lastPlatform){
		int heightoffset = 4 - (8 * Random.Range (0, 2));
		CreatePlatforms (platformNote, platformLength, lastPlatform, lastCreatedLength, heightoffset);
	}
}
