using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDownOnBeat : MonoBehaviour, Beater {

	public Text text;
	private int counter = 32;
	private bool active = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Beat(){
		if (active) {
			counter--;
			text.text = "" + counter;
		}
	}

	public void ResetAndStop(){
		counter = 32;
		text.text = "" + counter;
		active = false;
	}

	public void Continue(){
		active = true;
	}
}
