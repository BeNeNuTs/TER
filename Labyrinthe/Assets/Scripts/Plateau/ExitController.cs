using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitController : MonoBehaviour {

	public Text youWin;
	public float delayToShowScore = 2f;

	private GameController gameController;

	void Awake(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}

	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			youWin.enabled = true;
			gameController.LevelComplete();

			StartCoroutine("ShowScore", 2f);
		}
	}

	IEnumerator ShowScore(float time){
		yield return new WaitForSeconds (time);

		youWin.enabled = false;
		gameController.ShowScore ();
	}

}
