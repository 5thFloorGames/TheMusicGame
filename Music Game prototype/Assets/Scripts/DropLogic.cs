using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class DropLogic : MonoBehaviour, Quanter {

	public bool enable = true;
	private Camera2DFollow cameraSettings;
	private bool done = false;
	private float[] platformYFirstMarkers = 	{134f, 108f, 57f, -17f, -116f, -253f, -380f, -551f};
	private float[] platformYSecondMarkers = 	{131f, 100f, 44f, -35f, -136f, -278f, -412f, -585f};
	private float[] platformYBlocks = 			{128f, 92f, 31f, -53f, -156f, -303f, -444f}; // -619f
	private GameObject marker1;
	private GameObject marker2;
	private GameObject obstacle;
	private AudioClip boom;
	private AudioClip preDrop;
	private AudioSource audioSource;
	private GameObject player;
	private AudioMixer mixer;
	private BeatMatcher beat;
	private int beatCounter = 2; 

	void Awake(){
		cameraSettings = FindObjectOfType<Camera2DFollow> ();
		marker1 = Resources.Load<GameObject> ("Markers/Marker1");
		marker2 = Resources.Load<GameObject> ("Markers/Marker2");
		obstacle = Resources.Load<GameObject> ("Markers/Obstacle");
		boom = Resources.Load<AudioClip> ("Audio/Final/Boom");
		preDrop = Resources.Load<AudioClip> ("Audio/Final/Pre-drop");
		audioSource = GetComponentInParent<AudioSource> ();
		player = GameObject.FindWithTag("Player");
		mixer = audioSource.outputAudioMixerGroup.audioMixer;
		beat = FindObjectOfType<BeatMatcher> ();
	}

	// Use this for initialization
	void Start () {
		//audioSource.PlayOneShot(boom);
		if (enable) {
			float yOffset = transform.position.y - 140f;
			for (int i = 0; i < platformYFirstMarkers.Length; i++) {
				Instantiate (marker1, new Vector3 (transform.position.x, platformYFirstMarkers [i] + yOffset, 0f), Quaternion.identity);
			}
			for (int i = 0; i < platformYSecondMarkers.Length; i++) {
				Instantiate (marker2, new Vector3 (transform.position.x, platformYSecondMarkers [i] + yOffset, 0f), Quaternion.identity);
			}
			for (int i = 0; i < platformYBlocks.Length; i++) {
				Instantiate (obstacle, new Vector3 (transform.position.x, platformYBlocks [i] + yOffset, 0f), Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (enable && audioSource.isPlaying) {
			float distancefromCenter = Mathf.Abs (player.transform.position.x - transform.position.x) / 6.5f;

			mixer.SetFloat ("drymix", (100f - 55f * distancefromCenter) / 100f);
			mixer.SetFloat ("wetmix", (45f * distancefromCenter) / 100);
		}
	}

	public void Act(){
		player.SendMessage("Unfreeze");
		audioSource.Play();
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && !done && enable) {
			cameraSettings.damping = 0.05f;
			cameraSettings.lookAheadFactor = 0f;
			col.gameObject.GetComponent<Rigidbody2D> ().gravityScale = 0.6f;
			player.SendMessage("Freeze");
			player.SendMessage("FreeControl");
			player.SendMessage("Mute");
			audioSource.PlayOneShot(preDrop);
			player.transform.position = transform.position;
			done = true;
			beat.TriggerInXBars(this, 0);
		}
		if (col.tag == "Player" && !done && !enable) {
			cameraSettings.damping = 0.5f;
			cameraSettings.lookAheadFactor = 15f;
			col.gameObject.GetComponent<Rigidbody2D> ().gravityScale = 3f;
			done = true;
			FindObjectOfType<ScreenShake> ().jiggleCam (0.5f, 0.5f);
			player.SendMessage("AutoRun");
			player.SendMessage("LevelUp");
			audioSource.PlayOneShot(boom);
			col.SendMessage("UnMute");
		}
	}


}
