using UnityEngine;
using System.Collections;

public class ColorGradient : MonoBehaviour {
	
	private Color[] colors = {Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta};
	int choice = 1;
	Color lerped;
	float lerpProgress = 0.02f;
	
	// Use this for initialization
	void Start () {
		lerped = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
		
		lerped = Color.Lerp (lerped, colors [choice], lerpProgress);;
		GetComponent<Renderer>().material.color = lerped;

		if (sameColor(lerped, colors[choice])) {
			choice++;
			if(choice >= colors.Length){
				choice = 0;
			}
		}
		
	}
	
	bool closeEnough(float first, float second){
		if(first <= second + 0.1f && first >= second - 0.1f){
			return true;
		}
		return false;
	}
	
	bool sameColor(Color color1, Color color2){
		if(closeEnough(color1.b,color2.b)){
			if(closeEnough(color1.r,color2.r)){
				if(closeEnough(color1.g,color2.g)){
					return true;
				}
			}
		}
		
		return false;
	}
}