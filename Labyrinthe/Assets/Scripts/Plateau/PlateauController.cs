using UnityEngine;
using System.Collections;

public class PlateauController : MonoBehaviour {

	public float offset;
	public float speed;

	private Vector3 rotation;

	private GameController gameControllerScript;

	void Start(){
		GameObject gameController = GameObject.FindGameObjectWithTag("GameController");

		if(gameController != null)
			gameControllerScript = gameController.GetComponent<GameController>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(gameControllerScript != null)
			if(gameControllerScript.levelComplete)
				return;

		float h = Input.GetAxisRaw ("Vertical");
		float v = Input.GetAxisRaw ("Horizontal");

		// Set the movement vector based on the axis input.
		rotation += new Vector3 (h, 0f, -v) * speed * Time.smoothDeltaTime;

		clamp ();

		//rigidbody.MoveRotation(Quaternion.Euler(rotation));
		transform.localRotation = Quaternion.Euler(rotation);
	}

	public void ResetRotation(){
		rotation = Vector3.zero;
	}

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
