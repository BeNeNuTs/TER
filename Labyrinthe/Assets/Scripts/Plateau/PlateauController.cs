using UnityEngine;
using System.Collections;

public class PlateauController : MonoBehaviour {

	public float offset;
	public float speed;

	private Vector3 rotation;

	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxisRaw ("Vertical");
		float v = Input.GetAxisRaw ("Horizontal");

		// Set the movement vector based on the axis input.
		rotation += new Vector3 (h, 0f, -v) * speed * Time.deltaTime;

		clamp ();

		//rigidbody.MoveRotation(Quaternion.Euler(rotation));
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
