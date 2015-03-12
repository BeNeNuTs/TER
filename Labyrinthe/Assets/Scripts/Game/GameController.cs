using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

	public static string levelName;

	public Text levelText;
	public Text timeText;

	public GameObject tabScore;
	public GameObject pauseMenu;

	private bool levelComplete = false;
	private bool inPause = false;

	private float playerTime;
	private int playerDie = 0;

	void Start() {
		levelText.text += levelName;
		ResetDie ();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Cancel")) {
			TogglePauseMenu();
		}

		if (!levelComplete) {
			timeText.text = "Time : " + RoundValue (Time.timeSinceLevelLoad, 100f);
		}
	}

	private void TogglePauseMenu(){
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

	public void QuitTheGame(){
		if(Application.isEditor){
			Debug.Break();
		}else{
			Application.Quit ();
		}
	}

	public void NextLevel(){
		//TODO
		print ("Next Level");
	}
	
	public void Continue(){
		TogglePauseMenu();
	}

	public void ShowScore() {
		tabScore.GetComponent<TabScore> ().GenerateScore (playerTime, 0f);
		tabScore.SetActive (true);
	}

	public void LevelComplete(){
		levelComplete = true;
		playerTime = RoundValue (Time.timeSinceLevelLoad, 100f);
		timeText.text = "Time : " + playerTime;
	}

	public void IncreaseDie(){
		playerDie++;
	}

	public void ResetDie(){
		playerDie = 0;
	}

	public static float RoundValue(float num, float precision)
	{
		return Mathf.Floor(num * precision + 0.5f) / precision;
	}
}
