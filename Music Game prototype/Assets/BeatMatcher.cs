using UnityEngine;
using System.Collections;

public class BeatMatcher : MonoBehaviour {

	public ChangeColorOnBeat[] notifiables;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("ReportBeat", 0.46153846f, 0.46153846f);
		notifiables = FindObjectsOfType<ChangeColorOnBeat> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ReportBeat(){
		foreach (ChangeColorOnBeat notify in notifiables) {
			notify.Beat();
		}

	}
}
