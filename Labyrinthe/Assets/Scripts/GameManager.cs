using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;
	private Maze mazeInstance;

	private void BeginGame(){
		mazeInstance = Instantiate(mazePrefab) as Maze;
		StartCoroutine (mazeInstance.Generate());
	}
	
	private void RestartGame(){
		StopAllCoroutines ();
		Destroy(mazeInstance.gameObject);
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
