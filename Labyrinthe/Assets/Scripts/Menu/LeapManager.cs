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
	private enum possibleStates {MAIN_MENU = 0, LEVEL_MENU = 1, EDITOR_MENU = 2};
	private possibleStates currentState;

	// Use this for initialization
	void Start () {
		leap = new Controller ();
		leap.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		leap.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);
		leap.Config.SetFloat("Gesture.Swipe.MinVelocity", 350.0f);
		leap.Config.Save();
		cooldown = time;
		currentState = possibleStates.MAIN_MENU;
	}
	
	// Update is called once per frame
	void Update () {
		if (time >= cooldown) {
			foreach (Gesture g in leap.Frame().Gestures()) {
				if (g.Type == Gesture.GestureType.TYPE_SWIPE && g.State.Equals(Gesture.GestureState.STATESTOP)) { // On vérifie qu'on a un swipe terminé
					SwipeGesture swipe = new SwipeGesture (g);
					// Vers la gauche
					if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x < 0.0f){
						if(currentState == possibleStates.MAIN_MENU){
							mc.Play ();
							currentState = possibleStates.LEVEL_MENU;
							time = 0.0f; // Lorsque le swipe est détecté, on lance le cooldown
						} else if(currentState == possibleStates.EDITOR_MENU){
							mc.BackToMenu();
							currentState = possibleStates.MAIN_MENU;
							time = 0.0f; // Lorsque le swipe est détecté, on lance le cooldown
						} else {
							time = cooldown - 0.5f;
						}
					// Vers la droite
					} else if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x > 0.0f) {
						if(currentState == possibleStates.MAIN_MENU){
							mc.Editor();
							currentState = possibleStates.EDITOR_MENU;
							time = 0.0f; // Lorsque le swipe est détecté, on lance le cooldown
						} else if(currentState == possibleStates.LEVEL_MENU){
							mc.BackToMenu();
							currentState = possibleStates.MAIN_MENU;
							time = 0.0f; // Lorsque le swipe est détecté, on lance le cooldown
						} else {
							time = cooldown - 0.5f;
						}
					// Vers l'avant
					} else if (Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.x) && Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.y) && swipe.Direction.z < 0.0f) {
						if(currentState == possibleStates.MAIN_MENU){
							mc.Quit();
						}
					}
				}
			}
		} else {
			time += Time.deltaTime;
		}
	}
}
