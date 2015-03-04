using UnityEngine;
using System.Collections;
using Leap;

public class HandTracker : MonoBehaviour {

	Controller controller;
	HandList hands;
	public float speed;
	public float offset;
	public float speedPitch = 1.0f;
	public float speedRoll = 1.0f;
	Frame StartFrame;

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		StartFrame = controller.Frame ();
		controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture(Gesture.GestureType.TYPESWIPE);

	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame ();
		hands = frame.Hands;

		//Debug.Log (controller.IsGestureEnabled (Gesture.GestureType.TYPECIRCLE).ToString ());


		/*GestureList MyGestures = frame.Gestures(StartFrame);
		//Debug.Log (MyGestures.IsEmpty.ToString ());
		for (int index = 0; index < MyGestures.Count; index++) {
			//Debug.Log("Hello");
			Debug.Log(MyGestures[index].Type.ToString());
			if(MyGestures[index].Type.Equals(Gesture.GestureType.TYPECIRCLE))
			{
				//Time.timeScale = 0.0f;
				Debug.Log("Circle");
			}
			if(MyGestures[index].GetType().Equals(Gesture.GestureType.TYPESWIPE))
			{
				Time.timeScale = 1.0f;
				Debug.Log("Swip");
			}


		}*/

		ControlByTwoHands();
	
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
		
		rigidbody.MoveRotation (Quaternion.Euler (rotation));

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
		
		rigidbody.MoveRotation (Quaternion.Euler (rotation));
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
