using UnityEngine;
using System.Collections;

/** Classe permettant de gérer le chargement d'un niveau */
public class LevelManager : MonoBehaviour {

	private static LevelManager levelManager = null;
	public static int levelMaze = 1;

	public static int levelToLoad = -1;
	public static Level.LevelType levelTypeToLoad;

	/** Permet de définir que la classe LevelManger est un singleton */
	void Awake () {
		if(levelManager == null)
			levelManager = this;
		else if(levelManager != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad (gameObject);
	}

	/** Cette méthode peut etre appelé par n'importe quelle script afin de charger le niveau passé en paramètre */
	public static void setLevelToLoad (int level, Level.LevelType levelType) {
		levelToLoad = level;
		levelTypeToLoad = levelType;
		Application.LoadLevel(levelMaze);
	}

	/** Cette méthode est déclenché à chaque chargement de scène
	 *  Elle vérifie que l'on se trouve bien dans la scène main.unity
	 *  et que la variable levelToLoad a bien été passé afin de charger le niveau
	 */
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
