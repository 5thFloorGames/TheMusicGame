using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatMatcher : MonoBehaviour {

	public List<Beater> notifiables;
	private float startTime;
	private int counter = 0;
	private List<Quanter> callbacks;
	private List<SuperQuanter> quantCallbacks;
	private List<Quanter> oneOffs;

	// Use this for initialization
	void Awake () {
		quantCallbacks = new List<SuperQuanter> ();
		callbacks = new List<Quanter> ();
		oneOffs = new List<Quanter> ();
		
		notifiables = new List<Beater>();
		notifiables.AddRange(FindObjectsOfType<ChangeColorOnBeat> ());
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
		foreach (Quanter notify in oneOffs) {
			notify.Act();
		}
		oneOffs.Clear ();
	}

	public void ReportDoubleQuant(){
		foreach (Quanter notify in callbacks) {
			notify.Act();
		}
	}

	public void ReportQuant(){
		foreach (SuperQuanter notify in quantCallbacks) {
			notify.Quant();
		}
	}
	
	public void registerBeat(Beater callback){
		notifiables.Add (callback);
	}

	public void registerQuant(Quanter callback){
		callbacks.Add (callback);
	}

	public void registerActualQuant(SuperQuanter callback){
		quantCallbacks.Add (callback);
	}
	
	public void registerQuantOneOff(Quanter callback){
		oneOffs.Add (callback);
	}

}
