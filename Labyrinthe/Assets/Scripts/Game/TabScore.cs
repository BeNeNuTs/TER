using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TabScore : MonoBehaviour {

	public Text scoreText;
	public Text timeText;

	public GameObject [] fillStars;
	public float delayStar = 0.7f;

	private int starPoint = 10000;
	private AudioSource sound;

	void Start(){
		sound = GetComponent<AudioSource>();
	}

	public void GenerateScore(Level currentLevel, float time){
		int nbStars = 0;

		if(time < currentLevel.timeGold){
			nbStars = 3;
		}else if(time < currentLevel.timeSilver){
			nbStars = 2;
		}else if(time < currentLevel.timeBronze){
			nbStars = 1;
		}

		StartCoroutine(ShowStar(nbStars, delayStar));

		int score = nbStars * starPoint;
		scoreText.text += score;

		timeText.text += time;
	}

	IEnumerator ShowStar(int nbStars, float delay){
		yield return new WaitForSeconds(delay);

		for(int i = 0 ; i < nbStars ; i++){
			iTween.ScaleTo(fillStars[i], iTween.Hash("scale", new Vector3(1.2f,1.2f,1f), "time", 1f, "easetype", iTween.EaseType.easeOutElastic));
			sound.Play();
			yield return new WaitForSeconds(delay);
		}
	}
}
