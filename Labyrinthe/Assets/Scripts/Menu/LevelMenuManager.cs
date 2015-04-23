using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using Leap;


public class LevelMenuManager : MonoBehaviour {

	int nbLevels = 0;
	int nbLevelsSaved = 0;
	int nbTotalLevels = 0;
	public float RotationSpeed = 0.1f;
	public GameObject cubeLevel;
	List<GameObject> levels = new List<GameObject>();

	Controller controller;
	HandList hands;
	
	


	// Use this for initialization
	void Start () {

		controller = new Controller ();

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
			Debug.Log (levelNodes[i].Attributes["id"].InnerText);
			levels.Add(GameObject.Instantiate(cubeLevel) as GameObject);
			levels[levels.Count - 1].GetComponent<CubeLevel>().id = int.Parse(levelNodes[i].Attributes["id"].InnerText);
			levels[levels.Count - 1].GetComponent<CubeLevel>().type = Level.LevelType.Level;
	
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
			Debug.Log (levelNodes[i].Attributes["id"].InnerText);
			levels.Add(GameObject.Instantiate(cubeLevel) as GameObject);
			levels[levels.Count - 1].GetComponent<CubeLevel>().id = int.Parse(levelNodes[i].Attributes["id"].InnerText);
			levels[levels.Count - 1].GetComponent<CubeLevel>().type = Level.LevelType.SavedLevel;	
		}





		Debug.Log (nbLevels);
		Debug.Log (nbLevelsSaved);

		nbTotalLevels = nbLevels + nbLevelsSaved;


		//GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3(3,5,-11);

		createLevels();
	
	}


	//Création et disposition des levels (ici en cercle)
	void createLevels()
	{

		float rayon = nbTotalLevels;

		Camera.main.gameObject.transform.position = new Vector3 (0, rayon+10, 0);

		Vector3 CamPosition = Camera.main.gameObject.transform.position;
		CamPosition.Normalize();
		GameObject.Find("LevelTrigger").transform.position = new Vector3(CamPosition.x * rayon,CamPosition.y * rayon, CamPosition.z * rayon);	

	

		for(int i = 0; i< nbTotalLevels  ; i++)
		{
		

			double value = (double)i/(double)nbTotalLevels;
			double teta = (Math.PI*2)*value;
			double z = -Math.PI/2;
			float pX = (float)(rayon*Math.Sin(-Math.PI/2)*Math.Cos(teta));
			float pY = (float)(rayon*Math.Sin(-Math.PI/2)*Math.Sin((Mathf.PI*2)*value));
			float pZ = (float)(rayon*Math.Cos(z));

			levels[i].transform.position = new Vector3(pX,pY,pZ);

		}

		/*int nbLine = 0;
		for (int i = 0; i<4; i++)
		{
			if(i%4 == 0)
				nbLine++;

			levels[i].transform.position = new Vector3(15 * (i%4)/levels.Count, 10, nbLine*2);
		}*/

	}

	void moveCarousel()
	{
		Frame frame = controller.Frame ();
		hands = frame.Hands;

		Hand currentHand = hands[0];

		Vector3 axisRotation = new Vector3 (0, 0, 1);
		float angle = 0;

		if (currentHand.StabilizedPalmPosition.x < -15)
		{
			angle = 1;
		} 
		else if (currentHand.StabilizedPalmPosition.x > 15)
		{
			angle = -1;
		} 
			

		for(int cptLevel = 0 ; cptLevel<nbTotalLevels ; cptLevel++)
		{
			//levels[cptLevel].transform.Rotate(axisRotation, angle*RotationSpeed);
			levels[cptLevel].transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));

			levels[cptLevel].transform.RotateAround(new Vector3(0,0,0),axisRotation, angle*RotationSpeed);

		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		moveCarousel ();
	}




}
