using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatMatcher : MonoBehaviour {

	public List<Beater> notifiables;
	private float startTime;
	private int counter = 0;
	private float BPM = 120f;
	private float missThreshold = 0.0f;
	private float quantLength;
	private float beatLength;

	// Use this for initialization
	void Start () {
		beatLength = 60 / BPM;
		quantLength = beatLength / 4;

		print ("Beatlength:" + beatLength + " QuantLength:" + quantLength);
		notifiables = new List<Beater>();
		notifiables.AddRange(FindObjectsOfType<ChangeColorOnBeat> ());
		notifiables.AddRange(FindObjectsOfType<ChangeTextureOnBeat> ());
		notifiables.AddRange(FindObjectsOfType<CrushOnBeat> ());
		startTime = Time.time;

		StartCoroutine (TheBeat ());
	}

	IEnumerator TheBeat() {
		while (true) {
			yield return new WaitForSeconds (waitTimeForBeat());
			print (waitTimeForBeat());
			ReportBeat();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void ReportBeat(){
		counter++;
		foreach (Beater notify in notifiables) {
			notify.Beat();
		}
	}

	public float waitTimeForBeat(){
		float waitTime = beatLength - ((Time.time - startTime) % beatLength);
		return waitTime;
	}
	
	public float waitTimeForQuant(){
		float waitTime = quantLength - ((Time.time - startTime) % quantLength);
		return waitTime;
	}
}
