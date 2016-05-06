using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreCounter : MonoBehaviour {
	
	public Text text;
	private int counter = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	public void IncreaseScore(){
		counter++;
		text.text = "" + counter;
	}

	public void DecreaseScore(){
		counter--;
		text.text = "" + counter;
	}
}
