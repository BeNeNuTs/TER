using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.IO;

public class GameController : MonoBehaviour {

	public static Level currentLevel;

	public Text levelText;
	public Text timeText;

	public GameObject tabScore;
	public GameObject pauseMenu;

	[HideInInspector]
	public bool levelComplete = false;

	private bool inPause = false;
	private bool inGlobalView = false;

	private float playerTime;

	void Start() {
		levelText.text += currentLevel.name;
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Cancel")) {
			TogglePauseMenu();
		}else if (Input.GetButtonDown("Submit")) {
			ToggleView();
		}

		if (!levelComplete) {
			timeText.text = "Temps : " + RoundValue (Time.timeSinceLevelLoad, 100f);
		}
	}

	private void TogglePauseMenu(){
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

	//Méthode à appeler avec les boutons
	public void Quit(){
		GameController.QuitTheGame();
	}

	public static void QuitTheGame(){
		if(Application.isEditor){
			Debug.Break();
		}else{
			Application.Quit ();
		}
	}

	//Méthode à appeler avec les boutons
	public void ReplayB(){
		GameController.Replay();
	}

	public static void Replay(){
		Time.timeScale = 1f;
		LevelManager.setLevelToLoad(currentLevel.id, currentLevel.levelType); 
	}

	public void Back(){
		GameController.BackToMenu();
	}

	public static void BackToMenu(){
		Time.timeScale = 1f;
		Application.LoadLevel("menu");
	}

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
		int nextLevel = int.Parse(levelNodes[0].Attributes["id"].InnerText);
		for (int i = 0; i < levelNodes.Count - 1; i++)
		{
			if(levelNodes[i].Attributes["id"].InnerText == currentLevel.id.ToString()){
				nextLevel = int.Parse(levelNodes[i+1].Attributes["id"].InnerText);
				break;
			}
		}

		LevelManager.setLevelToLoad(nextLevel, currentLevel.levelType);
	}
	
	public void Continue(){
		TogglePauseMenu();
	}

	public void ShowScore() {
		tabScore.SetActive (true);
		tabScore.GetComponent<TabScore> ().GenerateScore (currentLevel, playerTime);
	}

	public void LevelComplete(){
		levelComplete = true;
		playerTime = RoundValue (Time.timeSinceLevelLoad, 100f);
		timeText.text = "Temps : " + playerTime;
	}

	public void ToggleView(){
		if(inGlobalView){
			SetLocalView();
		}else{
			SetGlobalView();
		}
	}

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

	private void SetLocalView(){
		if(iTween.Count(Camera.main.gameObject) > 0){
			iTween.Stop(Camera.main.gameObject);
		}

		inGlobalView = false;

		CameraFollow cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
		if(cameraFollowScript != null){
			cameraFollowScript.enabled = true;
		}
	}

	public static float RoundValue(float num, float precision)
	{
		return Mathf.Floor(num * precision + 0.5f) / precision;
	}
}
