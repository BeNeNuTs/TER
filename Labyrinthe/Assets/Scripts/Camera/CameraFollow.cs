using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public float smoothing = 5f;
	public Vector3 offset;

	private GameObject target;

	void FixedUpdate() {
		if(target == null){
			target = GameObject.FindGameObjectWithTag("Player") as GameObject;
		}

		Vector3 targetCamPos = target.transform.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}
