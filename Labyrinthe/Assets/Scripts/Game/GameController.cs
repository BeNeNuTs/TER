using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

	public void Back(){
		GameController.BackToMenu();
	}

	public static void BackToMenu(){
		Time.timeScale = 1f;
		Application.LoadLevel("menu");
	}

	public void NextLevel(){
		LevelManager.setLevelToLoad((currentLevel.id + 1) % 4, currentLevel.levelType);
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
		timeText.text = "Time : " + playerTime;
	}

	public void ToggleView(){
		if(inGlobalView){
			SetLocalView();
		}else{
			SetGlobalView();
		}
	}

	private void SetGlobalView(){
		iTween.Stop();

		inGlobalView = true;

		CameraFollow cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
		if(cameraFollowScript != null){
			cameraFollowScript.enabled = false;
		}
		EditorController.SetGlobalView(currentLevel.width, currentLevel.height);
	}

	private void SetLocalView(){
		iTween.Stop();

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
