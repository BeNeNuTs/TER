using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorController : MonoBehaviour {

	public Slider widthSlider;
	public Slider heightSlider;

	public Button saveButton;
	public Button solveButton;

	public GameObject mazeGameObject;

	private Maze maze;
	private PlateauController plateauScript;
	private DeadEndFilling deadEndScript;

	void Start(){
		maze = mazeGameObject.GetComponent<Maze>();
		plateauScript = mazeGameObject.GetComponent<PlateauController>();
		deadEndScript = GetComponent<DeadEndFilling>();
	}

	public void Generate(){
		if(!saveButton.IsInteractable()){
			saveButton.interactable = true;
		}
		if(!solveButton.IsInteractable()){
			solveButton.interactable = true;
		}

		//Supprimer l'ancien labyrinthe généré
		Transform [] child = maze.transform.GetComponentsInChildren<Transform>();
		foreach(Transform c in child){
			if(c != maze.transform)
				Destroy(c.gameObject);
		}
		//////////////////////////////////////

		ResetRotationLabyrinthe();

		//Récupérer le width & height des sliders et générer le labyrinthe
		int width = Mathf.FloorToInt(widthSlider.value);
		int height = Mathf.FloorToInt(heightSlider.value);
		maze.GenerateNoCoroutine(new IntVector2(width, height));

		SetGlobalView(width, height);		
	}

	public static void SetGlobalView(int width, int height){

		//Déplacer la caméra au bon endroit afin de voir le labyrinthe généré
		int max = Mathf.Max(width, height);
		iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", new Vector3(Camera.main.transform.position.x,max + max/4f,Camera.main.transform.position.z), "time", 2f));
		
	}

	public void Solve(){
		ResetRotationLabyrinthe();
		deadEndScript.deadEndFilling(maze, new IntVector2(0,0), new IntVector2(maze.size.x - 1, maze.size.z - 1));
	}

	public void Save(){
		Debug.Log("Saving...");
	}

	public void BackToMenu(){
		GameController.BackToMenu();
	}

	private void ResetRotationLabyrinthe(){
		//Remettre le labyrinthe en rotation (0,0,0)
		plateauScript.ResetRotation();
		maze.transform.rotation = Quaternion.Euler(0,0,0);
	}

}
