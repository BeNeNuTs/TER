using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorController : MonoBehaviour {

	public Slider widthSlider;
	public Slider heightSlider;

	public Button saveButton;

	public GameObject mazeGameObject;

	private Maze maze;
	private PlateauController plateauScript;

	void Start(){
		maze = mazeGameObject.GetComponent<Maze>();
		plateauScript = mazeGameObject.GetComponent<PlateauController>();
	}

	public void Generate(){
		if(!saveButton.IsInteractable()){
			saveButton.interactable = true;
		}

		//Supprimer l'ancien labyrinthe généré
		Transform [] child = maze.transform.GetComponentsInChildren<Transform>();
		foreach(Transform c in child){
			if(c != maze.transform)
				Destroy(c.gameObject);
		}
		//////////////////////////////////////

		//Remettre le labyrinthe en rotation (0,0,0)
		plateauScript.ResetRotation();
		maze.transform.rotation = Quaternion.Euler(0,0,0);

		//Récupérer le width & height des sliders et générer le labyrinthe
		int width = Mathf.FloorToInt(widthSlider.value);
		int height = Mathf.FloorToInt(heightSlider.value);
		maze.GenerateNoCoroutine(new IntVector2(width, height));

		//Déplacer la caméra au bon endroit afin de voir le labyrinthe généré
		int max = Mathf.Max(width, height);
		iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", new Vector3(Camera.main.transform.position.x,max + max/4f,Camera.main.transform.position.z), "time", 2f));
	}

	public void Save(){
		Debug.Log("Saving...");
	}

	public void BackToMenu(){
		GameController.BackToMenu();
	}

}
