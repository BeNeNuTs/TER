using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public Text bestScoreText;
	public Text bestTimeText;

	public GameObject [] fillStars;

	private bool alreadyGenerate = false;

	void Start(){
		if(!alreadyGenerate)
			GenerateBestScore();
	}

	public void GenerateBestScore(){
		alreadyGenerate = true;

		if(GameController.currentLevel.score != null)
			bestScoreText.text = "Meilleur score : " + GameController.currentLevel.score;
		else
			bestScoreText.text = "Meilleur score : -";

		if(GameController.currentLevel.time != null)
			bestTimeText.text = "Meilleur temps : " + GameController.currentLevel.time;
		else
			bestTimeText.text = "Meilleur temps : -";

		if(GameController.currentLevel.stars != null){
			for(int i = 0 ; i < GameController.currentLevel.stars ; i++){
				fillStars[i].SetActive(true);
			}
		}
	}

}
