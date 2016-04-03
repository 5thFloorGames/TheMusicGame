using UnityEditor;
using UnityEngine;

public class LevelBuilding : EditorWindow
{
	public Note platformNote;
	public int platformLength = 1;
	private GameObject platform;
	
	void Awake(){
		platform = Resources.Load<GameObject>("Platforms/MusicPlatform");
	}
	
	// Add menu item named "My Window" to the Window menu
	[MenuItem("LevelBuilding/Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(LevelBuilding));
	}
	
	void OnGUI()
	{
		GUILayout.Label ("Platform Settings", EditorStyles.boldLabel);
		platformNote = (Note)EditorGUILayout.EnumPopup (platformNote);
		EditorGUILayout.Space ();
		platformLength = EditorGUILayout.IntField (platformLength);
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Do it")) {
			CreatePlatforms();
		}
	}
	
	void CreatePlatforms(){
		if (Selection.activeGameObject != null) {
			if(Selection.activeGameObject.GetComponent<PlayOnTouch>() != null){
				platformNote = Selection.activeGameObject.GetComponent<PlayOnTouch>().note;
				platformLength = Mathf.FloorToInt(Selection.activeGameObject.transform.localScale.x / 10);
			}
			GameObject holder = new GameObject ("Platform " + platformNote);
			GameObject replaceable = Selection.activeGameObject;
			
			if(Selection.activeGameObject.GetComponent<PlayOnTouch>() != null){
				holder.transform.position = replaceable.transform.position;
				holder.transform.parent = replaceable.transform.parent.transform;
			} else {
				holder.transform.position = replaceable.transform.position;
				holder.transform.parent = replaceable.transform;
			}
			
			bool paired = platformLength % 2 == 0;
			
			for (float i = -Mathf.Floor(platformLength / 2); i <= Mathf.Floor(platformLength / 2); i++) {
				if(!paired || i != Mathf.Floor(platformLength / 2)){
					GameObject newPlatform = (GameObject)PrefabUtility.InstantiatePrefab (platform);
					newPlatform.GetComponent<PlayOnTouch>().SetNote(platformNote);
					if(paired){
						newPlatform.transform.position = holder.transform.position + new Vector3 ((i * 10) + 5, 0, 0);
					} else {
						newPlatform.transform.position = holder.transform.position + new Vector3 (i * 10, 0, 0);
					}
					newPlatform.transform.parent = holder.transform;
				}
			}
		}
	}
}