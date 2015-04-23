﻿using UnityEngine;
using System.Collections;
using System;
using Leap;

public class HandTracker : MonoBehaviour {

	public GameController gc;
	private Controller controller;
	private HandList hands;
	public float speed;
	public float offset;
	public float speedPitch = 1.0f;
	public float speedRoll = 1.0f;
	public float time;
	private float cooldown;
	private enum possibleStates {PLAYING = 0, PAUSE = 1};
	private possibleStates currentState;

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		currentState = possibleStates.PLAYING;
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 350.0f);
		controller.Config.Save();
		cooldown = time;
		time -= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
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
				Pause();
				break;
		}
	}

	private void Pause(){
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
						gc.TogglePauseMenu();
					} else {
						time = cooldown - 0.5f;
					}

				} else if(g.Type == Gesture.GestureType.TYPECIRCLE && g.State == Gesture.GestureState.STATE_STOP){
					CircleGesture circle = new CircleGesture (g);

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

	}

	private void ControlByOneHand()
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
	}


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