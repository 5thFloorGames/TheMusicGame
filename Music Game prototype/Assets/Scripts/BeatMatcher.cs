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
	private List<Quanter> callbacks;

	// Use this for initialization
	void Awake () {
		beatLength = 60 / BPM;
		quantLength = beatLength / 4;

		callbacks = new List<Quanter> ();

		notifiables = new List<Beater>();
		notifiables.AddRange(FindObjectsOfType<ChangeColorOnBeat> ());
		notifiables.AddRange(FindObjectsOfType<ChangeTextureOnBeat> ());
		notifiables.AddRange(FindObjectsOfType<CrushOnBeat> ());
		notifiables.AddRange(FindObjectsOfType<CountDownOnBeat> ());
		startTime = Time.time;
	}

	// Update is called once per frame
	void Update () {
	}

	public void ReportBeat(){
		foreach (Beater notify in notifiables) {
			notify.Beat();
		}
	}

	public void ReportDoubleQuant(){
		foreach (Quanter notify in callbacks) {
			notify.Act();
		}
	}

	public void registerQuant(Quanter callback){
		callbacks.Add (callback);
	}
}
