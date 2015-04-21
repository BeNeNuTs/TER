using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	private static LevelManager levelManager = null;
	public static int levelMaze = 1;

	public static int levelToLoad = -1;
	public static Level.LevelType levelTypeToLoad;
	
	void Awake () {
		if(levelManager == null)
			levelManager = this;
		else if(levelManager != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);
	}

	public static void setLevelToLoad (int level, Level.LevelType levelType) {
		levelToLoad = level;
		levelTypeToLoad = levelType;
		Debug.Log("LevelToLoad = " + level + " Level Type : " + levelTypeToLoad);
		Application.LoadLevel(levelMaze);
	}

	void OnLevelWasLoaded(int level) {
		if(level == levelMaze){
			if(levelToLoad < 0){
				Debug.LogError("Erreur levelToLoad < 0");
				return;
			}
			GameObject labManager = GameObject.Find("LabyrintheManager");
			if(labManager != null){
				labManager.GetComponent<LabyrintheManager>().GenerateLabyrinthe(levelToLoad, levelTypeToLoad);
			}else{
				Debug.LogError("Erreur : Impossible de trouver LabyrintheManager.");
			}

			levelToLoad = -1;
		}
	}
}
