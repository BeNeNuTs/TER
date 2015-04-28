﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class MenuController : MonoBehaviour {

	public GameObject menu;
	public GameObject levels;
	public GameObject editor;
	public GameObject carousel;

	public LevelMenuManager levelMenuManager;

	public float timeTransition = 1f;

	void Awake(){

		//Désactiver la vue des levels et la mettre à droite
		levels.SetActive(false);
		levels.transform.localPosition = new Vector3(Screen.width*2,0,0);

		//Désactiver la vue des editor et la mettre à gauche
		editor.SetActive(false);
		editor.transform.localPosition = new Vector3(-Screen.width*2,0,0);
	}


	public void Play(){
		Debug.Log("Play");

		//Mettre le menu a droite
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(-Screen.width * 2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));

		levels.SetActive(true);
		//Mettre les levels au milieu
		iTween.MoveTo(levels, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
		iTween.MoveTo(carousel, iTween.Hash("position", Vector3.zero, "time", timeTransition, "easetype", iTween.EaseType.easeInBack, "oncomplete", "ToggleCarousel", "oncompletetarget", this.gameObject));
		levelMenuManager.levelTrigger.transform.position -= new Vector3(LevelMenuManager.decalage,0,0);
	}

	public void Editor(){
		Debug.Log("Show editor");

		//Mettre le menu a gauche
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(Screen.width * 2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack, "oncomplete", "GoToEditor", "oncompletetarget", this.gameObject));
		
		//editor.SetActive(true);
		//Mettre les editor au milieu
		iTween.MoveTo(editor, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
	}

	public void BackToMenu(){
		//Remettre le menu au milieu
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));

		//Remettre les levels à droite
		iTween.MoveTo(levels, iTween.Hash("position", new Vector3(Screen.width * 2,Mathf.Floor(Screen.height/2),0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));
	
		//Remettre les editor à gauche
		iTween.MoveTo(editor, iTween.Hash("position", new Vector3(-Screen.width * 2,Mathf.Floor(Screen.height/2),0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));
	
		ToggleCarousel();
		levelMenuManager.levelTrigger.transform.position += new Vector3(LevelMenuManager.decalage,0,0);
		//Remettre carousel à droite
		iTween.MoveTo(carousel, iTween.Hash("position", new Vector3(LevelMenuManager.decalage,0,0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
	}

	private void ToggleCarousel(){
		levelMenuManager.readyToMove = !levelMenuManager.readyToMove;
	}

	public void GoToEditor(){
		Application.LoadLevel("editor");
	}

	public void Quit(){
		GameController.QuitTheGame();
	}
}
