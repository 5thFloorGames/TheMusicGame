using UnityEngine;
using System.Collections;

public class BeatMatcher : MonoBehaviour {

	public ChangeColorOnBeat[] notifiables;
	private float startTime;
	private int counter = 0;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("ReportBeat", 0.46153846f, 0.46153846f);
		notifiables = FindObjectsOfType<ChangeColorOnBeat> ();
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ReportBeat(){
		counter++;
		foreach (ChangeColorOnBeat notify in notifiables) {
			notify.Beat();
		}
	}
}
