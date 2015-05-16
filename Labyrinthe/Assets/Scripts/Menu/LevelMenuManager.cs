using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using Leap;

/** Classe permettant de gérer le carrousel avec le Leap Motion */
public class LevelMenuManager : MonoBehaviour {

	Controller controller;
	HandList hands;
	
	public int nbTotalLevels = 0;
	public float RotationSpeed = 0.1f, RotationRate = 0.5f, LeapFrame = 45.0f, timeToDeleteCube, time;
	public Animator textError;
	private int currentIndex = 0, nbLevels = 0, nbLevelsSaved = 0;
	private float nextRotation = 0.0f, cooldown;


	public Transform carousel;
	public GameObject cubeLevel, levelTrigger, deletionConfirmPopUp;
	public MenuController mc;
	List<GameObject> levels = new List<GameObject>();

	public bool readyToMove = false;
	private bool deleteConfirm = false;
	public static int decalage = 100;

	/** Initialise la classe en chargant et créant les différents niveaux dans le carrousel */
	void Start () {
		controller = new Controller ();
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);//Taille
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 300.0f);//Velocity
		controller.Config.Save();
		
		string path = LabyrintheManager.SearchPath (LabyrintheManager.folderLevels, "levels");
		path = path.Replace ("\\", "/");

		//Chargement des levels précréé
		
		XmlTextReader myXmlTextReader = LabyrintheManager.GetLevelXML();
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(myXmlTextReader);
		myXmlTextReader.Close();
		
		XmlNodeList levelNodes = xdoc.GetElementsByTagName("level");
		nbLevels = levelNodes.Count;
		
		for (int i = 0; i<levelNodes.Count; i++) 
		{
			levels.Add(GameObject.Instantiate(cubeLevel) as GameObject);
			levels[levels.Count - 1].GetComponent<CubeLevel>().id = int.Parse(levelNodes[i].Attributes["id"].InnerText);
			levels[levels.Count - 1].GetComponent<CubeLevel>().type = Level.LevelType.Level;
			levels[levels.Count - 1].GetComponent<CubeLevel>().setImage();
			
			if(levelNodes[i].Attributes["stars"].InnerText != "")
			{
				levels[levels.Count - 1].GetComponent<CubeLevel>().nbStars = int.Parse(levelNodes[i].Attributes["stars"].InnerText);
			}
			
			levels[levels.Count - 1].GetComponent<CubeLevel>().time.text = levelNodes[i].Attributes["time"].InnerText.ToString();
			levels[levels.Count - 1].GetComponent<CubeLevel>().nameText.text = levelNodes[i].Attributes["name"].InnerText.ToString();
			
			
		}

		//Chargement des levels crée par l'utilisateur
		myXmlTextReader = LabyrintheManager.GetSavedLevelXML();
		xdoc = new XmlDocument ();
		xdoc.Load(myXmlTextReader);
		myXmlTextReader.Close();
		levelNodes = xdoc.GetElementsByTagName("level");
		nbLevelsSaved = levelNodes.Count;
		
		for (int i = 0; i<levelNodes.Count; i++) 
		{
			levels.Add(GameObject.Instantiate(cubeLevel) as GameObject);
			levels[levels.Count - 1].GetComponent<CubeLevel>().id = int.Parse(levelNodes[i].Attributes["id"].InnerText);
			levels[levels.Count - 1].GetComponent<CubeLevel>().type = Level.LevelType.SavedLevel;	
			levels[levels.Count - 1].GetComponent<CubeLevel>().setImage();
			
			if(levelNodes[i].Attributes["stars"].InnerText != "")
			{
				levels[levels.Count - 1].GetComponent<CubeLevel>().nbStars = int.Parse(levelNodes[i].Attributes["stars"].InnerText);
			}
			
			levels[levels.Count - 1].GetComponent<CubeLevel>().time.text = levelNodes[i].Attributes["time"].InnerText.ToString();
			levels[levels.Count - 1].GetComponent<CubeLevel>().nameText.text = levelNodes[i].Attributes["name"].InnerText.ToString();
		}
		
		nbTotalLevels = nbLevels + nbLevelsSaved;
		
		createLevels();
		cooldown = time;
	}

	/** Permet de gérer les différents gestes effectué avec le Leap Motion */
	void Update () 
	{
		hands = controller.Frame ().Hands;

		if (hands.IsEmpty) 
			return;

		if (readyToMove) {
			moveCarousel ();
			if (time >= cooldown)
				handleGestures ();
			else
				time += Time.deltaTime;
		} else if (deleteConfirm) {
			if (time >= cooldown)
				handleGestures ();
			else
				time += Time.deltaTime;
		} else if (time < cooldown) {
			time += Time.deltaTime;
		}
	}

	// Fonction qui récupère l'objet dans le trigger de la scene et teste si c'est un sauvé ou non
	// Si oui, il est suppressible, sinon non
	private bool TriggerDeletable(){
		GameObject levelCaught = GameObject.Find("LevelTrigger");
		if (levelCaught.GetComponent<LevelLoader> ().type.Equals (Level.LevelType.SavedLevel))
			return true;
		return false;
	}

	// Fonction qui récupère le cube dans le trigger et retourne sa positino dans la liste level
	private int getIdInLevels(){
		GameObject levelCaught = GameObject.Find("LevelTrigger");
		int gameId = levelCaught.GetComponent<LevelLoader> ().id;
		Level.LevelType gameType = levelCaught.GetComponent<LevelLoader> ().type;
		
		for (int i = 0; i != nbTotalLevels; ++i) {
			if(levels[i].GetComponent<CubeLevel>().id == gameId && levels[i].GetComponent<CubeLevel>().type.Equals(gameType)){
				return i;
			}
		}
		
		return 0;
	}

	// Méthode qui gère tous les mouvements au leap dans la scène
	void handleGestures(){
		// Fermer le point pour lancer un niveau
		if(readyToMove && !hands.IsEmpty && (hands.Leftmost.SphereRadius <= 38 || hands.Rightmost.SphereRadius <= 38)){
			GameObject levelToLoad = GameObject.Find("LevelTrigger");
			LevelManager.setLevelToLoad(levelToLoad.GetComponent<LevelLoader>().id, levelToLoad.GetComponent<LevelLoader>().type);
		}

		// Check des swipes
		foreach (Gesture g in controller.Frame().Gestures()) {
			if (g.Type == Gesture.GestureType.TYPE_SWIPE && g.State.Equals (Gesture.GestureState.STATESTOP)) { // On vérifie qu'on a un swipe terminé
				SwipeGesture swipe = new SwipeGesture (g);
				// Swipe vers l'avant - demande de suppression de niveaux
				if (!deleteConfirm && (Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.x) && Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.y) && swipe.Direction.z < 0.0f)) {

					if (TriggerDeletable ()) {						// On ne suppr pas les niveaux de base du jeu
						if (nbTotalLevels > 0 && readyToMove) {		// On ne supprime que si on est pret à bouger et il y a au moins un niveau
							readyToMove = false;
							deleteConfirm = true;

							// Pop up de confirmation
							iTween.ScaleTo (deletionConfirmPopUp, iTween.Hash ("scale", new Vector3 (1.0f, 1.0f, 1.0f), "time", 1.5f, "easetype", iTween.EaseType.easeOutBack));
						}
					} else {
						textError.SetTrigger ("fade");		// Si on refuse de supprimer le niveau, on fait un retour
					}
				// Swipe vers la gauche - on annule la suppression
				} else if (deleteConfirm && (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x < 0.0f)) {
					deleteConfirm = false;
					iTween.ScaleTo (deletionConfirmPopUp, iTween.Hash ("scale", new Vector3 (0.0f, 0.0f, 0.0f), "time", 1.0f, "easetype", iTween.EaseType.easeInBack, "oncomplete", "notifyReadyToMove", "oncompletetarget", this.gameObject));
				// Swipe vers la droite - on accepte la suppression ou on fait un retour menu
				} else if (Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.y) && Math.Abs (swipe.Direction.x) > Math.Abs (swipe.Direction.z) && swipe.Direction.x > 0.0f) {				
					// Si on est en pop up de suppression ou en carroussel
					if (deleteConfirm) {
						time = cooldown-0.5f;
						readyToMove = false;
						deleteConfirm = false;
						iTween.ScaleTo (deletionConfirmPopUp, iTween.Hash ("scale", new Vector3 (0.0f, 0.0f, 0.0f), "time", 1.0f, "easetype", iTween.EaseType.easeInBack));
						iTween.ScaleTo (levelTrigger.GetComponent<LevelLoader> ().cube, iTween.Hash ("time", timeToDeleteCube, "easetype", iTween.EaseType.easeInBack, "scale", new Vector3 (0.0f, 0.0f, 0.0f), "oncomplete", "resizeCarrousel", "oncompletetarget", this.gameObject));
					} else if(readyToMove) {
						mc.BackToMenu ();
					} else {
						time = cooldown - 0.5f;
					}
				} else {
					time = 0.0f;
				}
			} else if(g.Type != Gesture.GestureType.TYPE_SWIPE){
				time = 0.0f;
			}
		}
	}

	// Méthode qui effectue la suppression et replace le carroussel lors d'une suppression
	public void resizeCarrousel(){
		// On récupère l'ID dans la liste des levels du niveau
		int id = getIdInLevels();

		// Récupération de la position de la caméra
		Vector3 CamPosition = Camera.main.gameObject.transform.position;
		CamPosition.Normalize ();

		//Suppression du level dans xml
		EditorController.RemoveLevel (levels[id].GetComponent<CubeLevel>().id);

		// On retire le cube de la liste, on décrémente le nombre de level et on met l'ID à jour
		levels.Remove (levelTrigger.GetComponent<LevelLoader> ().cube);
		--nbTotalLevels;
		id = id % nbTotalLevels;

		// On met à jour le rayon et on replace le trigger
		float rayon = nbTotalLevels;
		levelTrigger.transform.position = new Vector3(CamPosition.x * rayon,CamPosition.y * rayon, CamPosition.z * rayon);


		float angleCubeRotation = 360.0f / nbTotalLevels;
		Vector3 pointingVector = new Vector3 (CamPosition.x, CamPosition.y, CamPosition.z);
		pointingVector.Normalize ();

		// On replace chaque niveau au bon endroit
		for (int i = 0; i != nbTotalLevels; ++i) {
			iTween.MoveTo (levels [id], iTween.Hash ("x", pointingVector.x*rayon, "y", pointingVector.y*rayon, "z", pointingVector.z*rayon, "easetype", iTween.EaseType.easeOutBack, "time", 1.5f));
			id = ++id%nbTotalLevels;

			pointingVector = Quaternion.Euler(new Vector3(0, -angleCubeRotation, 0))*pointingVector;
		}

		// On replace la caméra au bon endroit
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", 0.0f, "y", 0.0f, "z", (float)-(rayon+10), "time", 1.5f, "oncomplete", "notifyReadyToMove", "oncompletetarget", this.gameObject));
	}

	// Méthode qui remet les valeurs des variables aux états de base
	public void notifyReadyToMove(){
		readyToMove = true;
		time = 0.0f;
		deleteConfirm = false;
	}

	//Création et disposition des levels (ici en cercle)
	void createLevels()
	{
		float rayon = nbTotalLevels;

		Camera.main.gameObject.transform.position = new Vector3 (0, 0, -(rayon+10));

		Vector3 CamPosition = Camera.main.gameObject.transform.position;
		CamPosition.Normalize();

		levelTrigger.transform.position = new Vector3(CamPosition.x * rayon,CamPosition.y * rayon, CamPosition.z * rayon);	
	
		//Disposition des levels en cercle
		for(int i = 0; i < nbTotalLevels  ; i++)
		{
			double value = (double)i/(double)nbTotalLevels;
			double teta = (Math.PI*2)*value;
			double z = -Math.PI/2;
			float pX = (float)(rayon*Math.Sin(-Math.PI/2)*Math.Cos(teta));
			float pZ = (float)(rayon*Math.Sin(-Math.PI/2)*Math.Sin((Mathf.PI*2)*value));
			float pY = (float)(rayon*Math.Cos(z));

			levels[i].transform.position = new Vector3(pX,pY,pZ);
			levels[i].transform.SetParent(carousel);
		}

		//Réajustement de la position des levels pour que le premier level soit aligné avec la caméra
		float angle = -1 *(Vector3.Angle (levels [0].transform.position, CamPosition));
		Vector3 axisRotation = new Vector3 (0, 1, 0);
		for(int cptLevel = 0 ; cptLevel<nbTotalLevels ; cptLevel++)
		{
			levels[cptLevel].transform.RotateAround(new Vector3(0,0,0),axisRotation, angle);
			levels[cptLevel].transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
		}
	
		Vector3 vectDecalage = new Vector3(decalage+rayon, 0, 0);
		carousel.transform.position += vectDecalage;
		levelTrigger.transform.position += vectDecalage;
	}

	/** Permet d'appliquer une rotation au carrousel en fonction du geste effectué */
	void moveCarousel()
	{
		Hand currentHand = hands[0];

		//Calcul de l'angle de rotation selon la position de la main
		float angle = 0;

		if (currentHand.StabilizedPalmPosition.x < -LeapFrame)
		{
			angle = 360/(float)nbTotalLevels ;
			currentIndex = ++currentIndex%nbTotalLevels;
		} 
		else if (currentHand.StabilizedPalmPosition.x > LeapFrame)
		{
			angle = -360/(float)nbTotalLevels;
			currentIndex = --currentIndex%nbTotalLevels;
		} 

		//Rotation du carousel	
		if(Time.time > nextRotation)
		{
			nextRotation = Time.time + RotationRate;
			iTween.RotateAdd(carousel.gameObject,iTween.Hash("y",angle,"time",RotationRate - 0.1, "space", Space.Self, "onupdate", "resetLevelRotation","onupdatetarget",this.gameObject));

			for(int cptLevel = 0 ; cptLevel<nbTotalLevels ; cptLevel++)
			{
				levels[cptLevel].transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			}
		}

	}
	
	/** Remet la rotation locale des niveaux à zéro */
	void resetLevelRotation()
	{
		for(int cptLevel = 0 ; cptLevel<nbTotalLevels ; cptLevel++)
		{
			levels[cptLevel].transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
		}
		
	}




	



}
