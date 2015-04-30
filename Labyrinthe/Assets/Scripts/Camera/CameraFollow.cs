using UnityEngine;
using System.Collections;

/** Cette classe permet à la caméra de suivre la bille dans le labyrinthe */
public class CameraFollow : MonoBehaviour {
	
	public float smoothing = 5f;
	public Vector3 offset;

	private GameObject target;

	/** Calcule la nouvelle position de la caméra à chaque update */
	void FixedUpdate() {
		if(target == null){
			target = GameObject.FindGameObjectWithTag("Player") as GameObject;
			return;
		}

		Vector3 targetCamPos = target.transform.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}
