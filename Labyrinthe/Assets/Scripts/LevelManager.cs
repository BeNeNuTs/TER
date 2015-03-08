using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	private static LevelManager levelManager = null;

	public static int levelToLoad = -1;
	
	void Awake () {
		if(levelManager == null)
			levelManager = this;
		else if(levelManager != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);
	}

	public static void setLevelToLoad (int level) {
		levelToLoad = level;
		Debug.Log("LevelToLoad = " + level);
		Application.LoadLevel("loadPieces");
	}

	void OnLevelWasLoaded(int level) {
		if(level == 1){
			if(levelToLoad < 0){
				Debug.LogError("Erreur levelToLoad < 0");
				return;
			}
			GameObject.Find("LabyrintheManager").GetComponent<LabyrintheManager>().GenerateLabyrinthe(levelToLoad);
			levelToLoad = -1;
		}
	}
}
