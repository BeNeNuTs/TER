using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

/** Classe permettant de gérer les différentes actions dans le menu */
public class MenuController : MonoBehaviour {

	public GameObject menu;
	public GameObject levels;
	public GameObject editor;
	public GameObject carousel;
	public LeapManager scriptMenu;

	public LevelMenuManager levelMenuManager;

	public float timeTransition = 1f;

	/** Initialise les positions des différents menus */
	void Awake(){

		//Désactiver la vue des levels et la mettre à droite
		levels.SetActive(false);
		levels.transform.localPosition = new Vector3(Screen.width*2,0,0);
		scriptMenu.enabled = true;
		levelMenuManager.enabled = true;
		//Désactiver la vue des editor et la mettre à gauche
		/*editor.SetActive(false);
		editor.transform.localPosition = new Vector3(-Screen.width*2,0,0);*/
	}

	/** Décale les menus pour afficher le menus jouer avec le carrousel */
	public void Play(){
		//Mettre le menu a droite
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(-Screen.width * 2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));

		levels.SetActive(true);
		//Mettre les levels au milieu
		iTween.MoveTo(levels, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
		iTween.MoveTo(carousel, iTween.Hash("position", Vector3.zero, "time", timeTransition, "easetype", iTween.EaseType.easeInBack, "oncomplete", "ToggleCarousel", "oncompletetarget", this.gameObject));
		levelMenuManager.levelTrigger.transform.position -= new Vector3(LevelMenuManager.decalage+levelMenuManager.nbTotalLevels,0,0);
		scriptMenu.enabled = false;
		levelMenuManager.enabled = true;
	}

	/** Décale les menus pour afficher la scène de l'éditeur */
	public void Editor(){
		//Mettre le menu a gauche
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(Screen.width * 2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
		//Mettre le titre à gauche
		iTween.MoveTo(transform.GetChild(0).gameObject, iTween.Hash("position", new Vector3(Screen.width * 2, transform.GetChild(0).transform.position.y, 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack, "oncomplete", "GoToEditor", "oncompletetarget", this.gameObject));

		scriptMenu.enabled = false;
		//editor.SetActive(true);
		//Mettre les editor au milieu
		//iTween.MoveTo(editor, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
	}

	/** Permet de revenir au menu principal */
	public void BackToMenu(){
		levelMenuManager.enabled = false;
		//Remettre le menu au milieu
		iTween.MoveTo(menu, iTween.Hash("position", new Vector3(Screen.width/2, Mathf.Floor(Screen.height/2), 0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));

		//Remettre les levels à droite
		iTween.MoveTo(levels, iTween.Hash("position", new Vector3(Screen.width * 2,Mathf.Floor(Screen.height/2),0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));
	
		//Remettre les editor à gauche
		iTween.MoveTo(editor, iTween.Hash("position", new Vector3(-Screen.width * 2,Mathf.Floor(Screen.height/2),0), "time", timeTransition, "easetype", iTween.EaseType.easeInCubic));
	
		ToggleCarousel();
		levelMenuManager.levelTrigger.transform.position += new Vector3(LevelMenuManager.decalage+levelMenuManager.nbTotalLevels,0,0);
		//Remettre carousel à droite
		iTween.MoveTo(carousel, iTween.Hash("position", new Vector3(LevelMenuManager.decalage+levelMenuManager.nbTotalLevels,0,0), "time", timeTransition, "easetype", iTween.EaseType.easeInBack));
		scriptMenu.enabled = true;
	}

	/** Permet d'activer/désactiver la rotation du labyrinthe */
	private void ToggleCarousel(){
		levelMenuManager.readyToMove = !levelMenuManager.readyToMove;
	}

	/** Charge la scène de l'éditeur */
	public void GoToEditor(){
		Application.LoadLevel("editor");
	}

	/** Permet de quitter le jeu */
	public void Quit(){
		GameController.QuitTheGame();
	}
}
