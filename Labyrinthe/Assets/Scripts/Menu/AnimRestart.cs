using UnityEngine;
using System.Collections;

/** Classe permettant d'animer le logo de recommencer */
public class AnimRestart : MonoBehaviour {

	public float rotation;
	public float time;
	
	/** Initialise l'animation du restart */
	void Start () {
		iTween.RotateAdd (gameObject, iTween.Hash ("z", rotation, "time", time, "islocal", true, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.easeInOutQuart, "ignoretimescale", true));
	}
}
