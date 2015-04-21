using UnityEngine;
using System.Collections;

public class AnimArrow : MonoBehaviour {

	public enum Direction { X, Y }

	public float magnitude;
	public float speed;

	public Direction direction;
	// Use this for initialization
	void Start () {

		if(direction == Direction.X)
			iTween.MoveTo (gameObject, iTween.Hash ("position", new Vector3 (transform.localPosition.x - magnitude, transform.localPosition.y, 0.0f), "time", speed, "islocal", true, "looptype", "pingpong", "easetype", iTween.EaseType.easeInQuart, "ignoretimescale", true));
		else{
			iTween.MoveTo (gameObject, iTween.Hash ("position", new Vector3 (transform.localPosition.x, transform.localPosition.y - magnitude, 0.0f), "time", speed, "islocal", true, "looptype", "pingpong", "easetype", iTween.EaseType.easeInQuart, "ignoretimescale", true));
			iTween.ScaleTo (gameObject, iTween.Hash ("scale", new Vector3 (transform.localScale.x - 0.2f, transform.localScale.y - 0.2f, 0.0f), "time", speed, "islocal", true, "looptype", "pingpong", "easetype", iTween.EaseType.easeInQuart, "ignoretimescale", true));
		}
	}
}
