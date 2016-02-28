using UnityEngine;
using System.Collections;

public class BeatMatcher : MonoBehaviour {

	public ChangeColorOnBeat[] notifiables;
	private float startTime;
	private int counter = 0;
	private float BPM = 130f;
	private float missThreshold = 0.02f;
	private float quantLength;

	// Use this for initialization
	void Start () {
		float beatLength = 60 / BPM;
		quantLength = beatLength / 4;

		print ("Beatlength:" + beatLength + " QuantLength:" + quantLength);
		InvokeRepeating ("ReportBeat", beatLength, beatLength);
		notifiables = FindObjectsOfType<ChangeColorOnBeat> ();
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		print (waitTimeForQuant());
	}

	void ReportBeat(){
		counter++;
		foreach (ChangeColorOnBeat notify in notifiables) {
			notify.Beat();
		}
	}

	public float waitTimeForQuant(){
		float waitTime = quantLength - ((Time.time - startTime) % quantLength);
		if (waitTime < missThreshold) {
			waitTime = 0;
		}
		return waitTime;
	}
}
