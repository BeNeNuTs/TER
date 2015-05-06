using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/** Classe permettant de savoir si le joueur à atteint la sortie dans un labyrinthe */
public class ExitController : MonoBehaviour {

	public float delayToShowScore = 2f;

	private GameController gameController;

	/** Initialise la sortie en récupèrant le script GameController */
	void Awake(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}

	// Détecte si le joueur est entré dans la sortie */
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player" && !gameController.levelComplete) {
			gameController.LevelComplete();

			StartCoroutine("ShowScore", delayToShowScore);
		}
	}

	/** Appel l'affichage du score après un délai de time */
	IEnumerator ShowScore(float time){
		yield return new WaitForSeconds (time);
		gameController.ShowScore ();
	}

}
