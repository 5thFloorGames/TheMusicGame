using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatMatcher : MonoBehaviour {

	public List<Beater> notifiables;
	private float startTime;
	private int barCounter = 0;
	private List<Quanter> callbacks;
	private List<SuperQuanter> quantCallbacks;
	private List<Quanter> oneOffs;
	private List<Quanter> barOneOffs;
	private Dictionary<Quanter, int> barWaitOneOffs;
	private Dictionary<Quanter, int> beatWaitOneOffs;

	// Use this for initialization
	void Awake () {
		quantCallbacks = new List<SuperQuanter> ();
		callbacks = new List<Quanter> ();
		oneOffs = new List<Quanter> ();
		barOneOffs = new List<Quanter> ();
		barWaitOneOffs = new Dictionary<Quanter, int> ();
		beatWaitOneOffs = new Dictionary<Quanter, int> ();
		
		notifiables = new List<Beater>();
		notifiables.AddRange(FindObjectsOfType<ChangeColorOnBeat> ());
		notifiables.AddRange(FindObjectsOfType<CountDownOnBeat> ());
		startTime = Time.time;
	}

	public void ReportBeat(){
		bool clear = false;
		foreach (Beater notify in notifiables) {
			notify.Beat();
		}
		foreach (Quanter notify in oneOffs) {
			notify.Act();
		}
		if (barCounter % 4 == 0) {
			foreach (Quanter notify in barOneOffs) {
				notify.Act();
			}
			foreach(Quanter notify in barWaitOneOffs.Keys){
				if(notify == null){
					print ("NULL THING FOUND");
					clear = true;
				} else {
					if (barCounter > barWaitOneOffs[notify]){
						notify.Act();
						clear = true;
					}
				}
			}
			if(clear){
				barWaitOneOffs.Clear();
			}
			barOneOffs.Clear();
		}
		foreach(Quanter notify in beatWaitOneOffs.Keys){
			if(notify == null){
				print ("NULL THING FOUND");
				clear = true;
			} else {
				if (barCounter >= beatWaitOneOffs[notify]){
					notify.Act();
					clear = true;
				}
			}
		}
		if(clear){
			beatWaitOneOffs.Clear();
		}
		barCounter++;
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
	
	public void registerBeatOneOff(Quanter callback){
		oneOffs.Add (callback);
	}

	public void registerBarOneOff(Quanter callback){
		barOneOffs.Add (callback);
	}

	public void TriggerInXBars(Quanter callback, int bars){
		barWaitOneOffs.Add (callback, barCounter + bars * 4);
	}

	public void TriggerInXBeats(Quanter callback, int beats){
		beatWaitOneOffs.Add (callback, barCounter + beats);
	}

}
