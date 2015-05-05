using UnityEngine;
using System.Collections;
using Leap;
using System;

public class LeapManager : MonoBehaviour {

	public MenuController mc;	// Permet de naviguer entre les menus
	public float time;

	private Controller leap;
	private Pointable zone;
	private float cooldown;

	// Use this for initialization
	void Start () {
		leap = new Controller ();
		leap.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		leap.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);
		leap.Config.SetFloat("Gesture.Swipe.MinVelocity", 350.0f);
		leap.Config.Save();
		cooldown = time;
	}
	
	// Update is called once per frame
	void Update () {
		if (time >= cooldown) {
			foreach (Gesture g in leap.Frame().Gestures()) {
				if (g.Type == Gesture.GestureType.TYPE_SWIPE && g.State.Equals(Gesture.GestureState.STATESTOP)) { // On vérifie qu'on a un swipe terminé
					SwipeGesture swipe = new SwipeGesture (g);
					// Vers la gauche
					if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x < 0.0f){
						mc.Play();
						time = 0.0f;
					// Vers la droite
					} else if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x > 0.0f) {
						mc.Editor();
						time = 0.0f;
					// Vers l'avant
					} else if (Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.x) && Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.y) && swipe.Direction.z < 0.0f) {
						mc.Quit ();
					} else {
						time = 0.0f;
					}
				} else if(g.Type != Gesture.GestureType.TYPE_SWIPE){
					time = 0.0f;
				}
			}
		} else {
			time += Time.deltaTime;
		}
	}
}
