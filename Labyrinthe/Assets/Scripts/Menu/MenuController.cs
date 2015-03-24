using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class MenuController : MonoBehaviour {

	public GameObject menu;
	public GameObject levels;
	public GameObject options;

	public float timeTransition = 1f;

	void Awake(){
		Debug.Log("Screen : width = " + Screen.width + " height = " + Screen.height);

		//Désactiver la vue des levels et la mettre à droite
		levels.SetActive(false);
		levels.transform.position = new Vector3(Screen.width * 2,Mathf.Floor(Screen.height/2),0);

		//Désactiver la vue des options et la mettre à gauche
		options.SetActive(false);
		options.transform.position = new Vector3(-Screen.width * 2,Mathf.Floor(Screen.height/2),0);

	}


	public void Play(){
		Debug.Log("Play");

		//Mettre le menu a droite
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(-Screen.width * 2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));

		levels.SetActive(true);
		//Mettre les levels au milieu
		iTween.MoveTo(levels, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
	}

	public void Options(){
		Debug.Log("Show Options");

		//Mettre le menu a gauche
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(Screen.width * 2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
		
		options.SetActive(true);
		//Mettre les options au milieu
		iTween.MoveTo(options, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
	}

	public void BackToMenu(){
		//Remettre le menu au milieu
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));

		//Remettre les levels à droite
		iTween.MoveTo(levels, iTween.Hash("position", new Vector3(Screen.width * 2,Mathf.Floor(Screen.height/2),0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));
	
		//Remettre les options à gauche
		iTween.MoveTo(options, iTween.Hash("position", new Vector3(-Screen.width * 2,Mathf.Floor(Screen.height/2),0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));
	}

	public void GoToEditor(){
		Application.LoadLevel("editor");
	}

	public void Quit(){
		GameController.QuitTheGame();
	}
}
