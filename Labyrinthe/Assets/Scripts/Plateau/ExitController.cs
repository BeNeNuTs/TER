using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitController : MonoBehaviour {

	public float delayToShowScore = 2f;

	private GameController gameController;

	void Awake(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}

	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			gameController.LevelComplete();

			StartCoroutine("ShowScore", delayToShowScore);
		}
	}

	IEnumerator ShowScore(float time){
		yield return new WaitForSeconds (time);
		gameController.ShowScore ();
	}

}
