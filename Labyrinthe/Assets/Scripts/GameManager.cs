using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;

	private void BeginGame(){
		//StartCoroutine (mazePrefab.GenerateWithCoroutine());
		mazePrefab.GenerateNoCoroutine ();
	}
	
	private void RestartGame(){
		//StopAllCoroutines ();
		Destroy(mazePrefab.gameObject);
		BeginGame();
	}

	// Use this for initialization
	private void Start () {
		BeginGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			RestartGame ();
		}
	}
}
