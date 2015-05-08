using UnityEngine;
using System.Collections;
using System;
using Leap;

/** Classe permettant de gérer les interactions Leap Motion durant la partie */
public class HandTracker : MonoBehaviour {

	public GameController gc;
	public GameObject menuPause;
	private Controller controller;
	private HandList hands;
	public float speed, offset, speedPitch = 1.0f, time;
	private float cooldown;
	private enum possibleStates {PLAYING = 0, PAUSE = 1, LEVEL_COMPLETE = 2, WAITING = 3};
	private possibleStates currentState;
	private bool unZoomed;

	float oldPitch;
	public Vector3 rot;
	public Vector3 rotation;

	public bool HandMode = false;

	/** Initialise la classe Leap Motion */
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
		speed = 1000;
		offset = 15;
		rot = new Vector3 (0, 0, 0);
	}

	/** Détecte les gestes de l'utilisateur */
	void Update () {
		// Si on a fini le jeu on lance le menu de fin
		if (currentState != possibleStates.LEVEL_COMPLETE && gc.levelComplete) {
			controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, true);
			controller.EnableGesture(Gesture.GestureType.TYPESWIPE, true);
			currentState = possibleStates.LEVEL_COMPLETE;
		}

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
				LevelComplete();
			break;

			case possibleStates.WAITING:
				if(controller.Frame ().Hands.Count == 2){
					menuPause.SetActive(false);
					currentState = possibleStates.PLAYING;
					gc.handsChecker = true;
				}else if(!menuPause.activeSelf){
					menuPause.SetActive(true);
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
						//controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
						//controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
						time = cooldown;
						GameController.BackToMenu();
						
					} else if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x > 0.0f) {
						// Si vers la droite, on passe au level suivant
						//controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
						//controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
						time = cooldown;
						gc.NextLevel();
						return;
					} else {
						time = cooldown - 0.5f;
					}
					
				} else if(g.Type == Gesture.GestureType.TYPECIRCLE && g.State == Gesture.GestureState.STATE_STOP){
					// Si cercle, rejouer le niveau
					//controller.EnableGesture(Gesture.GestureType.TYPECIRCLE, false);
					//controller.EnableGesture(Gesture.GestureType.TYPESWIPE, false);
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
		if (hands.Count < 2) {
			//Debug.Log("NUL");
			return;
		} 

		Hand LeftHand = hands.Leftmost;
		Hand RightHand = hands.Rightmost;


		float vX = RightHand.PalmPosition.x - LeftHand.PalmPosition.x;
		float vY = LeftHand.PalmPosition.y - RightHand.PalmPosition.y;
		
		Vector3 vBalance = new Vector3(vX , vY, 0);
		float vAngle = Vector3.Angle(new Vector3 (1, 0, 0), vBalance);
		float vAngleR = Mathf.PI * vAngle / 180;
		
		float pitch = LeftHand.Direction.Pitch - Mathf.PI*25/180f;
		//float pitchD = 180*pitch/Mathf.PI;


		if ((oldPitch > 0 && pitch>0 && oldPitch > pitch) || (oldPitch < 0 && pitch < 0 && oldPitch < pitch)) 
		{
			pitch = -1 * (oldPitch - pitch); 
		}

		float angleZ = 0;
		//float angleX = 0;

		
		//angleZ  = (RightHand.PalmPosition.y - LeftHand.PalmPosition.y)/ Math.Abs(RightHand.PalmPosition.y - LeftHand.PalmPosition.y) ;

		int width = GameController.currentLevel.width;
		int height = GameController.currentLevel.height;

		if (RightHand.PalmPosition.y > LeftHand.PalmPosition.y) 
		{ 
			angleZ = 1;
			rotation = new Vector3 (-1*pitch, 0, vAngleR*Math.Max(width,height)/5 )* speed * Time.deltaTime;
		}
		else
		{
			angleZ = -1;
			rotation = new Vector3 (-1*pitch, 0, -vAngleR*Math.Max(width,height)/5 )  * speed * Time.deltaTime;
		}

		/*if (-1 * pitch * speedPitch > 0) 
		{
			angleX = 1;
		} 
		else 
		{
			angleX = -1;
		}*/

		rot += new Vector3 (-1*pitch, 0, angleZ*vAngleR); //* Time.smoothDeltaTime;
		rot = clamp (rot);

		if (HandMode) 
		{        

			transform.localRotation = (Quaternion.Euler (rotation));
		} 
		else 
		{
			transform.localRotation = (Quaternion.Euler (rot));
		}


		oldPitch = pitch;

		
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
