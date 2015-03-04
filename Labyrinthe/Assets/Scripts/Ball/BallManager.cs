using UnityEngine;
using System.Collections;

public class BallManager : MonoBehaviour {


	Vector3 StartPos;
	// Use this for initialization
	void Start () {
		StartPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = transform.position;
		if (pos.y < -20 || pos.y > 20)
		{
			transform.position = StartPos;
			transform.rigidbody.velocity = new Vector3(0,0,0);
		}
	}
}
