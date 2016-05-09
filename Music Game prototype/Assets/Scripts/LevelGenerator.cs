using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	private GameObject platform;
	private GameObject killBlock;
	private GameObject halfKillBlock;
	private GameObject lastCreated;
	public GameObject downLevelBackground;
	private int lastCreatedLength = 1;
	private int lastOffSet = 0;
	private Note lastNote;
	private bool lastDouble = false;
	private ComposingLogic composingLogic;
	private int levelCounter = 0;
	private GameObject launcherTrigger;
	private GameObject theDrop;
	private GameObject bigRing;
	private GameObject smallRing;
	private GameObject tutorial;
	private bool postDrop = false;
	private bool girlTanks = true;
	private float lowestY;
	private static bool tutorialEnabled = true;
	private float levelProgress = 0f;
	private int levelLength = 75;

	void Awake(){
		platform = (GameObject)Resources.Load ("Platforms/MusicPlatform");
		killBlock = (GameObject)Resources.Load ("Platforms/KillBlock");
		halfKillBlock = (GameObject)Resources.Load ("Platforms/HalfKillBlock");
		launcherTrigger = Resources.Load<GameObject>("LauncherTrigger");
		bigRing = Resources.Load<GameObject>("Rings/BigRing");
		smallRing = Resources.Load<GameObject>("Rings/SmallRing");
		theDrop = Resources.Load<GameObject>("Drop");
		tutorial = Resources.Load<GameObject> ("Tutorial");
		composingLogic = GetComponent<ComposingLogic> ();
	}

	// Use this for initialization
	void Start () {
		int tunnel = Random.Range (10, 25);
		int drop = Random.Range (25, 35);
		int tunnel2 = Random.Range (45, 60);
		//drop = 1;
		//tunnel = 1;
		int platformLength;


		for (int i = 0; i < levelLength; i++) {
			print((0.2f + 0.2f * levelProgress));
			levelProgress = (float)i/levelLength;
			girlTanks = true;
			if(i == 0){
				if(tutorialEnabled){
					GameObject tutorialObject = (GameObject)Instantiate(tutorial, transform.position + Vector3.right * 20f, Quaternion.identity);
					lastCreated = tutorialObject.transform.FindChild("LastPlatform").gameObject;
					lastCreatedLength = 6;
				} else {
					CreatePlatforms (Note.i, Random.Range (3, 6), transform, false, true);
				}
			} else {
				if(i == tunnel || i == tunnel2){
					if(i == tunnel){
						girlTanks = false;
					}
					Transform start = lastCreated.transform;
					int lastLength = lastCreatedLength;
					//Instantiate(Resources.Load<GameObject>("LauncherPauser"), lastCreated.transform.position + ((lastCreatedLength - 1.5f) * 5f) * Vector3.right, Quaternion.identity);
					Instantiate(launcherTrigger,lastCreated.transform.position + (lastCreatedLength * 5f) * Vector3.right, Quaternion.identity);
					CreatePlatforms(composingLogic.randomHighNote(),25,start, lastLength, 4, true, false);
					CreatePlatforms(Note.i,25,start, lastLength, 0, true, false);
					CreatePlatforms(composingLogic.randomLowNote(),25,start, lastLength, -4, true, true);
					GameObject shutdownTrigger = (GameObject)Instantiate(launcherTrigger,lastCreated.transform.position + Vector3.right * 110f, Quaternion.identity);
					shutdownTrigger.GetComponent<ActivateLauncherOnTouch>().type = TriggerType.Deactivate;
				} else if(i == drop){
					GameObject dropObject = (GameObject)Instantiate(theDrop, lastCreated.transform.position + (lastCreatedLength + 1.5f - 1) * (Vector3.right * 5f) + Vector3.right * 3.15f + Vector3.up * 0.75f, Quaternion.identity);
					lastCreated = dropObject.transform.FindChild("Landing").gameObject;
					Instantiate(downLevelBackground, new Vector3(lastCreated.transform.position.x, lastCreated.transform.position.y, 9.1875f), Quaternion.identity);
					lastCreatedLength = 3;
					postDrop = true;
				} else {
					if(lastCreatedLength > 1){
						platformLength = Random.Range (1, 5);
					} else {
						platformLength = Random.Range (2, 5);
					}
					if(RandomChance(0.2f + 0.2f * levelProgress)){
						CreateDoublePlatform(composingLogic.nextNote(lastNote, levelCounter),platformLength,lastCreated.transform);
						lastDouble = true;
					} else {
						CreatePlatforms(composingLogic.nextNote(lastNote, levelCounter),platformLength,lastCreated.transform, false, true);
						lastDouble = false;
					}
					levelCounter++;
					if (levelCounter > 3) {
						levelCounter = 0;
					}
				}
			}
		}
	}

	private bool RandomChance(float percent){
		return Random.Range (0f, 1f) < percent;
	}

	void CreatePlatforms(Note platformNote, int platformLength, Transform lastPlatform, int lastLength, int heightOffset, bool parallelplatforms, bool killBlock){
		GameObject holder = new GameObject ("Platform " + platformNote);
		GameObject parent = gameObject;
		holder.transform.parent = parent.transform;

		holder.transform.position = lastPlatform.position + (lastLength + platformLength - 1) * (Vector3.right * 5f) + Vector3.right;
		
		holder.transform.position += Vector3.up * heightOffset;
		
		bool even = platformLength % 2 == 0;

		float iStart = -Mathf.Floor (platformLength / 2);
		float iEnd = Mathf.Floor (platformLength / 2);

		for (float i = iStart; i <= iEnd; i++) {
			if(!even || i != Mathf.Floor(platformLength / 2)){
				GameObject newPlatform = (GameObject)Instantiate (platform);
				newPlatform.GetComponent<PlayOnTouch>().SetNote(platformNote);
				if(even){
					newPlatform.transform.position = holder.transform.position + new Vector3 ((i * 10) + 5, 0, 0);
				} else {
					newPlatform.transform.position = holder.transform.position + new Vector3 (i * 10, 0, 0);
				}
				newPlatform.transform.parent = holder.transform;
				if(RandomChance(0.2f + 0.2f * levelProgress) && girlTanks){
					newPlatform.AddComponent<CreateBlock>();
				} else if(RandomChance(0.1f + 0.2f * levelProgress) && postDrop){
					if(parallelplatforms){
						Instantiate(bigRing,newPlatform.transform.position  + (-1) * heightOffset * Vector3.up, Quaternion.Euler(90f,90f,0));
					} else {
						Instantiate(smallRing,newPlatform.transform.position, Quaternion.Euler(90f,90f,0));
					}
				}
				if (RandomChance(0.03f)) {
					//newPlatform.AddComponent<ShootOnTouch>();
				}

				if(killBlock && platformLength != 1){
					if(i == iStart){
						AddHalfKillBlock(newPlatform, 2.5f);
					} else if((i == iEnd && !even) || (i == iEnd -1 && even)){
						AddHalfKillBlock(newPlatform, -2.5f);
					} else {
						AddKillBlock(newPlatform);
					}
				}

				lastCreated = holder;
				lastNote = platformNote;
				lastCreatedLength = platformLength;
				lastOffSet = heightOffset;
			}
		}

	}

	void AddHalfKillBlock(GameObject holder, float xOffset){
		GameObject newBlock = (GameObject)Instantiate (halfKillBlock, holder.transform.position + Vector3.down * 5f + xOffset * Vector3.right, Quaternion.identity);
		newBlock.transform.parent = holder.transform;
	}

	void AddKillBlock(GameObject holder){
		GameObject newBlock = (GameObject)Instantiate (killBlock, holder.transform.position + Vector3.down * 5f, Quaternion.identity);
		newBlock.transform.parent = holder.transform;
	}
	
	void CreatePlatforms(Note platformNote, int platformLength, Transform lastPlatform, bool parallelPlatforms, bool killBlock){
		int heightoffset = 4 - (8 * Random.Range (0, 2));
		if (lastDouble) {
			heightoffset = 4;
		}
		CreatePlatforms (platformNote, platformLength, lastPlatform, lastCreatedLength, heightoffset, parallelPlatforms, killBlock);
	}

	void CreateDoublePlatform(Note platformNote, int platformLength, Transform lastPlatform){
		int lastLength = lastCreatedLength;
		CreatePlatforms (platformNote, platformLength, lastPlatform, lastLength, 4, true, false);
		CreatePlatforms (platformNote, platformLength, lastPlatform, lastLength, -4, true, true);
	}

	public void DisableTutorial(){
		tutorialEnabled = false;
	}

	public void EnableTutorial(){
		tutorialEnabled = true;

	}
}
