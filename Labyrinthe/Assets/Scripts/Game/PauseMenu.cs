using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public Text bestScoreText;
	public Text bestTimeText;

	public GameObject [] fillStars;

	/** A l'appel du Start, génère l'affichage des meilleurs scores */
	void Start(){
		GenerateBestScore();
	}

	/** Méthode permettant de récupèrer les meilleurs scores du joueur */
	public void GenerateBestScore(){

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
