using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using Leap;


public class LevelMenuManager : MonoBehaviour {

	Controller controller;
	HandList hands;

	int nbLevels = 0, nbLevelsSaved = 0, nbTotalLevels = 0;
	public float RotationSpeed = 0.1f, RotationRate = 0.5f, LeapFrame = 45.0f, timeToDeleteCube;
	private int currentIndex = 0;
	private float nextRotation = 0.0f;


	public Transform carousel;
	public GameObject cubeLevel, levelTrigger;
	List<GameObject> levels = new List<GameObject>();

	public bool readyToMove = false;
	public static int decalage = 100;

	// Use this for initialization
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
			//Debug.Log (levelNodes[i].Attributes["id"].InnerText);
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
			//Debug.Log (levelNodes[i].Attributes["id"].InnerText);
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
		
		//Debug.Log (nbLevels);
		//Debug.Log (nbLevelsSaved);
		
		nbTotalLevels = nbLevels + nbLevelsSaved;
		//GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3(3,5,-11);
		
		createLevels();
	}

	// Update is called once per frame
	void Update () 
	{
		Frame frame = controller.Frame ();
		hands = frame.Hands;

		if (hands.IsEmpty) 
		{
			return;
		}

		if(readyToMove){
			moveCarousel ();
			handleGestures ();
		}
	}

	void handleGestures()
	{
		if(!hands.IsEmpty && (hands.Leftmost.SphereRadius <= 38 || hands.Rightmost.SphereRadius <= 38)) 
		{
			GameObject levelToLoad = GameObject.Find("LevelTrigger");
			LevelManager.setLevelToLoad(levelToLoad.GetComponent<LevelLoader>().id, levelToLoad.GetComponent<LevelLoader>().type);
		}

		foreach (Gesture g in controller.Frame().Gestures()) {
			if (g.Type == Gesture.GestureType.TYPE_SWIPE && g.State.Equals(Gesture.GestureState.STATESTOP)) { // On vérifie qu'on a un swipe terminé
				SwipeGesture swipe = new SwipeGesture (g);

				if(Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.x) && Math.Abs (swipe.Direction.z) > Math.Abs (swipe.Direction.y) && swipe.Direction.z < 0.0f) {
					//TODO checker si ça passe avec 1-2 cubes

					// On empeche la suppression si il n'y a plus de cube et si aucun iTween n'est en cours
					if(nbTotalLevels > 0 && readyToMove){
						readyToMove = false;
						iTween.ScaleTo(levelTrigger.GetComponent<LevelLoader>().cube, iTween.Hash("time", timeToDeleteCube, "easetype", iTween.EaseType.easeInBack, "scale", new Vector3(0.0f, 0.0f, 0.0f), "oncomplete", "resizeCarrousel", "oncompletetarget", this.gameObject));
					}
				}
			}
		}
	}

	public void resizeCarrousel(){
		// On récupère l'ID dans la liste des levels du niveau et on récupère un voisin direct
		int id = levelTrigger.GetComponent<LevelLoader> ().id;
		float angle = Vector3.Angle (Camera.main.gameObject.transform.position, levels [id].transform.position);

		// Récupération de la position de la caméra
		Vector3 CamPosition = Camera.main.gameObject.transform.position;
		CamPosition.Normalize ();

		// On retire le cube de la liste et on décrémente le nombre de level
		levels.Remove (levelTrigger.GetComponent<LevelLoader> ().cube);
		--nbTotalLevels;

		// On met à jour le rayon et on replace le trigger
		float rayon = nbTotalLevels;
		levelTrigger.transform.position = new Vector3 (CamPosition.x * rayon, CamPosition.y * rayon, CamPosition.z * rayon);

		//TODO  Lancer la suppression dans le XML

		double z = -Math.PI / 2;
		for (int i = 0; i != nbTotalLevels; ++i) {
			double value = (double)i / (double)nbTotalLevels;
			double theta = (Math.PI * 2) * value;

			float pX = (float)(rayon * Math.Sin (-Math.PI / 2) * Math.Cos (theta));
			float pZ = (float)(rayon * Math.Sin (-Math.PI / 2) * Math.Sin ((Mathf.PI * 2) * value));
			float pY = (float)(rayon * Math.Cos (z));

			iTween.MoveTo (levels [i], iTween.Hash ("x", pX, "y", pY, "z", pZ, "easetype", iTween.EaseType.easeInOutBack, "time", 1.5f));
		}

		iTween.RotateAdd (carousel.gameObject, iTween.Hash ("y", angle, "time", RotationRate - 0.1, "space", Space.Self, "onupdate", "resetLevelRotation", "onupdatetarget", this.gameObject));
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", 0.0f, "y", 0.0f, "z", (float)-(rayon+10), "time", 1.5f, "oncomplete", "notifyReadyToMove", "oncompletetarget", this.gameObject));
	}

	// Méthode qui remet readyToMove à vrai
	public void notifyReadyToMove(){
		readyToMove = true;
	}

	//Création et disposition des levels (ici en cercle)
	void createLevels()
	{
		float rayon = nbTotalLevels;

		Camera.main.gameObject.transform.position = new Vector3 (0, 0, -(rayon+10));

		Vector3 CamPosition = Camera.main.gameObject.transform.position;
		CamPosition.Normalize();

		//levelTrigger = GameObject.Find("LevelTrigger");
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

			//Allumage des étoiles
			//levels[cptLevel].
		}
	
		Vector3 vectDecalage = new Vector3(decalage+rayon, 0, 0);
		carousel.transform.position += vectDecalage;
		levelTrigger.transform.position += vectDecalage;
	}

	void moveCarousel()
	{
		Hand currentHand = hands[0];

		//Calcul de l'angle de rotation selon la position de la main
		//Vector3 axisRotation = new Vector3 (0, 1, 0);
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
			//carousel.localRotation = Quaternion.Euler(new Vector3(0,0,0));

			for(int cptLevel = 0 ; cptLevel<nbTotalLevels ; cptLevel++)
			{
				levels[cptLevel].transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			}
		}

	}
	
	void resetLevelRotation()
	{
		for(int cptLevel = 0 ; cptLevel<nbTotalLevels ; cptLevel++)
		{
			levels[cptLevel].transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
		}
		
	}




	



}
