using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;
	private DeadEndFilling DEF;
	
	private void BeginGame(){
		//StartCoroutine (mazePrefab.GenerateWithCoroutine());
		mazePrefab.GenerateNoCoroutine ();
		//StartCoroutine (DEF.deadEndFillingWithCoroutine (mazePrefab));
		DEF.deadEndFilling (mazePrefab);
	}
	
	private void RestartGame(){
		//StopAllCoroutines ();
		Destroy(mazePrefab.gameObject);
		BeginGame();
	}

	// Use this for initialization
	private void Start () {
		DEF = GetComponent<DeadEndFilling> ();
		BeginGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			RestartGame ();
		}
	}
}
