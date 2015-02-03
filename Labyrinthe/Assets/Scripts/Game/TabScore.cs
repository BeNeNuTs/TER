using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TabScore : MonoBehaviour {

	public Text scoreText;

	public Text timeText;
	public Text dieText;

	public void GenerateScore(float time, float die){
		timeText.text += time;
		dieText.text += die;
	}
}
