using UnityEngine;
using System.Collections;
using System;
using Leap;

public class HandTracker : MonoBehaviour {

	public GameController gc;
	private Controller controller;
	private HandList hands;
	public float speed, offset, speedPitch = 1.0f, time;
	private float cooldown;
	private enum possibleStates {PLAYING = 0, PAUSE = 1, LEVEL_COMPLETE = 2, WAITING = 3};
	private possibleStates currentState;
	private bool unZoomed;

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		currentState = possibleStates.WAITING;
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);				// Un swipe doit faire au moins 20cm
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 350.0f); 			// Un swipe doit aller à au moins 350 mm/s
		controller.Config.SetFloat("Gesture.Circle.MinArc", (float) Math.PI * 2.0f);// Un cercle doit faire un tour complet
		controller.Config.Save();
		cooldown = time;
		time -= 0.5f;
		unZoomed = false;
	}

	// Update is called once per frame
	void Update () {
		// Si on a fini le jeu on lance le menu de fin
		if (gc.levelComplete) 
			currentState = possibleStates.LEVEL_COMPLETE;

		hands = controller.Frame ().Hands;

		// Détection de la pause
		if (currentState == possibleStates.PLAYING) {
			if(hands.IsEmpty){
				currentState = possibleStates.PAUSE;
				controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, true);
				controller.EnableGesture(Gesture.GestureType.TYPESWIPE, true);
				gc.TogglePauseMenu();
			}
		}

		// Appel des fonctions correspondantes à l'état
		switch (currentState) {
			case possibleStates.PLAYING:
				ControlByTwoHands();
				break;
				
			case possibleStates.PAUSE:
				StartCoroutine("updatePause");
				break;

			case possibleStates.LEVEL_COMPLETE:
				controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, true);
				controller.EnableGesture(Gesture.GestureType.TYPESWIPE, true);
				LevelComplete();
			break;

			case possibleStates.WAITING:
				if(controller.Frame ().Hands.Count == 2){
					currentState = possibleStates.PLAYING;
					gc.handsChecker = true;
				}
			break;
		}
	}

	// Coroutine pour que la pause continue de fonctionner avec un time scale à 0
	IEnumerator updatePause(){
		if (time >= cooldown) {
			foreach (Gesture g in controller.Frame().Gestures()) {
				if(g.Type == Gesture.GestureType.TYPE_SWIPE && g.State == Gesture.GestureState.STATE_STOP){
					SwipeGesture swipe = new SwipeGesture (g);
					
					// swipe vers la gauche ou vers la droite
					if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x < 0.0f){
						// Si vers la gauche, retour au menu
						controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
						controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
						time = cooldown;
						GameController.BackToMenu();
						
					} else if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x > 0.0f) {
						// Si vers la droite, on enlève la pause
						controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
						controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
						time = cooldown;
						currentState = possibleStates.PLAYING;
					} else {
						time = cooldown - 0.5f;
					}
					
				} else if(g.Type == Gesture.GestureType.TYPECIRCLE && g.State == Gesture.GestureState.STATE_STOP){
					// Si cercle, rejouer le niveau
					controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
					controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
					time = cooldown;
					GameController.Replay();
				}
			}
		} else {
			time += 0.1f;
		}
		if (currentState == possibleStates.PAUSE) {
			yield return null;
		} else {
			gc.TogglePauseMenu();
			StopCoroutine("updatePause");
		}
	}

	// Lorsque le niveau est terminé, on gère le menu de fin
	void LevelComplete(){
		if (time >= cooldown) {
			foreach (Gesture g in controller.Frame().Gestures()) {
				if(g.Type == Gesture.GestureType.TYPE_SWIPE && g.State == Gesture.GestureState.STATE_STOP){
					SwipeGesture swipe = new SwipeGesture (g);
					
					// swipe vers la gauche ou vers la droite
					if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x < 0.0f){
						// Si vers la gauche, retour au menu
						controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
						controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
						time = cooldown;
						GameController.BackToMenu();
						
					} else if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x > 0.0f) {
						// Si vers la droite, on passe au level suivant
						controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
						controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
						time = cooldown;
						gc.NextLevel();
						return;
					} else {
						time = cooldown - 0.5f;
					}
					
				} else if(g.Type == Gesture.GestureType.TYPECIRCLE && g.State == Gesture.GestureState.STATE_STOP){
					// Si cercle, rejouer le niveau
					controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
					controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
					time = cooldown;
					GameController.Replay();
				}
			}
		} else {
			time += Time.deltaTime;
		}
	}

	// Méthode qui gère le plateau lors du jeu
	private void ControlByTwoHands()
	{
		Hand LeftHand = hands.Leftmost;
		Hand RightHand = hands.Rightmost;

		float vX = RightHand.PalmPosition.x - LeftHand.PalmPosition.x;
		float vY = LeftHand.PalmPosition.y - RightHand.PalmPosition.y;
		//float vZ = LeftHand.StabilizedPalmPosition.z - RightHand.StabilizedPalmPosition.z;

		Vector3 vBalance = new Vector3(vX , vY, 0);
		float vAngle = Vector3.Angle(new Vector3 (1, 0, 0), vBalance);


		float pitch = (LeftHand.Direction.Pitch + RightHand.Direction.Pitch)/2;
		float pitchD = 180*pitch/Mathf.PI - 25;


		Vector3 rotation;

		if (RightHand.PalmPosition.y > LeftHand.PalmPosition.y) 
		{ 
			rotation = new Vector3 (-1*pitchD*speedPitch, 0, vAngle) * speed * Time.deltaTime;
				
		}
		else
		{
			rotation = new Vector3 (-1*pitchD*speedPitch, 0, -vAngle)  * speed * Time.deltaTime;
		}

		//rotation = clamp (rotation);
		if (hands.Count < 2) {
			//Debug.Log("NUL");
			rotation = new Vector3 (0,0,0);
		} 
		
		transform.localRotation = (Quaternion.Euler (rotation));

		if (hands.Leftmost.SphereRadius <= 40) {
			if(!unZoomed){
				unZoomed = true;
				gc.ToggleView();
			}
		} else {
			if(unZoomed){
				unZoomed = false;
				gc.ToggleView();
			}
		}

	}

	// Méthode censée gérer un controle à une main, jugé peu convénient
	/*private void ControlByOneHand()
	{
		Hand firstHand = hands[0];
		
		float pitch = firstHand.Direction.Pitch;
		float yaw = firstHand.Direction.Yaw;
		float roll = firstHand.PalmNormal.Roll;
		
		float pitchD = 180*pitch/3.14f;
		float rollD = 180*roll/3.14f;
		float yawD = 0;//180*yaw/3.14f;
		
		Debug.Log ("Pitch" + pitch);
		Debug.Log ("Yaw" + yaw);
		Debug.Log ("Roll" + roll);
		
		Vector3 rotation = new Vector3 (-1*pitchD*speedPitch, yawD, rollD*speedRoll)  * speed * Time.deltaTime;

		if (!firstHand.IsValid) {
			Debug.Log("NUL");
			rotation = new Vector3 (0,0,0);
		} 
		
		GetComponent<Rigidbody>().MoveRotation (Quaternion.Euler (rotation));
	}*/


	// Fonction qui retourne la rotation du labyrinthe
	private Vector3 clamp(Vector3 rotation) 
	{
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

		return rotation;
	}
	

}
