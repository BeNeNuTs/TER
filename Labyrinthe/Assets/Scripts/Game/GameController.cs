using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.IO;

/** La classe GameController contient les fonctions qui permettent de gérer la partie en cours */
public class GameController : MonoBehaviour {

	public static Level currentLevel;

	public Text levelText;
	public Text timeText;

	public GameObject tabScore;
	public GameObject pauseMenu;

	[HideInInspector]
	public bool levelComplete = false;
	[HideInInspector]
	public bool handsChecker = false;

	private bool inPause = false;
	private bool inGlobalView = false;

	private float playerTime;

	/** Initialise le nom de Labyrinthe courant */
	void Start() {
		levelText.text += currentLevel.name;;
	}

	/** Détecte si le joueur met le jeu en pause ou change la vue de la caméra */
	void Update () {

		if (Input.GetButtonDown("Cancel")) {
			TogglePauseMenu();
		}else if (Input.GetButtonDown("Submit")) {
			ToggleView();
		}

		if (!levelComplete && handsChecker) {
			playerTime += Time.deltaTime;
			playerTime = RoundValue (playerTime, 100f);
			timeText.text = "Temps : " + playerTime;
		}
	}

	/** Active ou désactive la pause */
	public void TogglePauseMenu(){
		if(levelComplete)
			return;

		if (inPause) {
			inPause = false;
			Time.timeScale = 1f;
			pauseMenu.SetActive (false);
		} else {
			inPause = true;
			Time.timeScale = 0f;
			pauseMenu.SetActive(true);
		}
	}

	/** Méthode permettant de quitter le jeu
	 *  A utiliser avec les boutons (car on ne peut pas utiliser les méthodes static)
	 */
	public void Quit(){
		GameController.QuitTheGame();
	}

	/** Méthode static permettant de quitter le jeu */
	public static void QuitTheGame(){
		if(Application.isEditor){
			Debug.Break();
		}else{
			Application.Quit ();
		}
	}

	/** Méthode permettant de recommencer un niveau
	 *  A utiliser avec les boutons (car on ne peut pas utiliser les méthodes static)
	 */
	public void ReplayB(){
		GameController.Replay();
	}

	/** Méthode static permettant de recommencer un niveau */
	public static void Replay(){
		Time.timeScale = 1f;
		LevelManager.setLevelToLoad(currentLevel.id, currentLevel.levelType); 
	}

	/** Dans la scène du menu, permet de retourner au menu principal 
	 *	(Jouer, Editeur, Quitter)
	 */
	public void Back(){
		GameController.BackToMenu();
	}

	/** Méthode static qui permet dans la scène du menu, de retourner au menu principal 
	 *	(Jouer, Editeur, Quitter)
	 */
	public static void BackToMenu(){
		Time.timeScale = 1f;
		Application.LoadLevel("menu");
	}

	/** Recherche le niveau suivant par rapport au niveau actuel et charge la scène */
	public void NextLevel(){
		XmlTextReader myXmlTextReader;
		if(currentLevel.levelType == Level.LevelType.Level)
			myXmlTextReader = LabyrintheManager.GetLevelXML();
		else
			myXmlTextReader = LabyrintheManager.GetSavedLevelXML();
		
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(myXmlTextReader);
		
		myXmlTextReader.Close();
		
		XmlNodeList levelNodes = xdoc.GetElementsByTagName("level");
		int nextLevel = -1;
		Level.LevelType levelType = currentLevel.levelType;
		for (int i = 0; i < levelNodes.Count - 1; i++)
		{
			if(levelNodes[i].Attributes["id"].InnerText == currentLevel.id.ToString()){
				nextLevel = int.Parse(levelNodes[i+1].Attributes["id"].InnerText);
				break;
			}
		}

		if(nextLevel == -1){
			nextLevel = int.Parse(levelNodes[0].Attributes["id"].InnerText);

			if(currentLevel.levelType == Level.LevelType.Level){
				myXmlTextReader = LabyrintheManager.GetSavedLevelXML();
			}
			else{
				myXmlTextReader = LabyrintheManager.GetLevelXML();
			}

			xdoc = new XmlDocument();
			xdoc.Load(myXmlTextReader);
			myXmlTextReader.Close();
			
			levelNodes = xdoc.GetElementsByTagName("level");
			if(levelNodes.Count > 0){
				nextLevel = int.Parse(levelNodes[0].Attributes["id"].InnerText);
				if(levelType == Level.LevelType.Level)
					levelType = Level.LevelType.SavedLevel;
				else
					levelType = Level.LevelType.Level;
			}
		}

		if(nextLevel >= 0){
			Debug.Log ("Niveau trouvé: " + nextLevel);
			LevelManager.setLevelToLoad(nextLevel, levelType);
		}else{
			Debug.LogError ("Erreur nextLevel < 0");
		}
	}

	/** Retire le menu pause et continue la partie */
	public void Continue(){
		TogglePauseMenu();
	}

	/** Affiche les scores en fin de partie */
	public void ShowScore() {
		tabScore.SetActive (true);
		tabScore.GetComponent<TabScore> ().GenerateScore (currentLevel, playerTime);
	}

	/** Permet de spécifier que le niveau est terminé ( le joueur a gagné )
	 * 	Cette méthode est appelé par la classe ExitController
	 */
	public void LevelComplete(){
		levelComplete = true;
		//playerTime = RoundValue (Time.timeSinceLevelLoad, 100f);
		timeText.text = "Temps : " + playerTime;
	}

	/** Change la vue de la caméra Local/Global */
	public void ToggleView(){
		if(inGlobalView){
			SetLocalView();
		}else{
			SetGlobalView();
		}
	}

	/** Change la vue de la caméra en Global View */
	private void SetGlobalView(){
		if(iTween.Count(Camera.main.gameObject) > 0){
			iTween.Stop(Camera.main.gameObject);
		}

		inGlobalView = true;

		CameraFollow cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
		if(cameraFollowScript != null){
			cameraFollowScript.enabled = false;
		}
		EditorController.SetGlobalView(currentLevel.width, currentLevel.height);
	}

	void GlobalView(int width, int height)
	{
		//Déplacer la caméra au bon endroit afin de voir le labyrinthe généré
		int max = Mathf.Max(width/5, height/5);
		Vector3 newPos = new Vector3(Camera.main.transform.position.x,max,Camera.main.transform.position.z);
		
		if(Camera.main.transform.position != newPos)
			iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", newPos, "time", 2f));
	}
	
	/** Change la vue de la caméra en Local View */
	private void SetLocalView(){
		if(iTween.Count(Camera.main.gameObject) > 0){
			iTween.Stop(Camera.main.gameObject);
		}
		
		inGlobalView = false;
		
		CameraFollow cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
		//cameraFollowScript.offset = new Vector3(0,0, currentLevel.height/5);
		if(cameraFollowScript != null){
			cameraFollowScript.enabled = true;
		}
	}

	/** Arrondi un float avec <precision> chiffre après la virgule */
	public static float RoundValue(float num, float precision)
	{
		return Mathf.Floor(num * precision + 0.5f) / precision;
	}
}
