using UnityEngine;
using System.Collections;

/** Classe permettant de gérer la rotation du labyrinthe au clavier (utilisé dans la scène Editeur) */
public class PlateauController : MonoBehaviour {

	public float offset;
	public float speed;

	private Vector3 rotation;

	private GameController gameControllerScript;

	/** Initialise la classe en récupèrant le script GameController */
	void Start(){
		GameObject gameController = GameObject.FindGameObjectWithTag("GameController");

		if(gameController != null)
			gameControllerScript = gameController.GetComponent<GameController>();
	}

	// A chaque update, récupère les entrées clavier et incline le labyrinthe en conséquence */
	void FixedUpdate () {
		if(gameControllerScript != null)
			if(gameControllerScript.levelComplete)
				return;

		float h = Input.GetAxisRaw ("Vertical");
		float v = Input.GetAxisRaw ("Horizontal");

		rotation += new Vector3 (h, 0f, -v) * speed * Time.smoothDeltaTime;

		clamp ();

		transform.localRotation = Quaternion.Euler(rotation);
	}

	/** Remet la rotation du labyrinthe à zéro */
	public void ResetRotation(){
		rotation = Vector3.zero;
	}

	/** Permet de borner la rotation du labyrinthe pour qu'elle ne dépasse pas les valeurs de l'offset */
	private void clamp() {
		if (rotation.x > offset) {
			rotation = new Vector3(offset, 0f, rotation.z);
		}else if (rotation.x < -offset) {
			rotation = new Vector3(-offset, 0f, rotation.z);
		}

		if (rotation.z > offset) {
			rotation = new Vector3(rotation.x, 0f, offset);
		}else if (rotation.z < -offset) {
			rotation = new Vector3(rotation.x, 0f, -offset);
		}
	}
}
