using UnityEngine;
using System.Collections;

public class AnimRestart : MonoBehaviour {

	public float rotation;
	public float time;
	
	// Use this for initialization
	void Start () {
		iTween.RotateAdd (gameObject, iTween.Hash ("z", rotation, "time", time, "islocal", true, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.easeInOutQuart, "ignoretimescale", true));
	}
}
