using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class LabyrintheManager : MonoBehaviour {

	public static string folderDocs = "/Documents";
	public static string folderLevels = "/Levels";
	public static string folderPieces = "/Pieces";
	public static string folderSave = "/Save";

	public static float sizePiece = 3.5f;

	private Piece [] listPieces;
	private Level currentLevel;
	private GameObject plateau;

	public GameObject bille;
	public GameObject exit;
	
	void Awake(){
		DontDestroyOnLoad (gameObject);
	
		plateau = GameObject.Find ("Plateau");
		currentLevel = null;

		LoadPieces ();
		LoadLevel (1);

		GenerateLabyrinthe ();
	}

	public void LoadPieces(){
		CheckIfFolderDocsExist ();

		string path = SearchPath (folderPieces, "pieces");
		if (path == "") {
			Debug.LogError("Impossible d'ouvrir le fichier pieces.xml");
			return;
		}

		path = path.Replace ("\\", "/");

		List<Piece> listPiecesTmp = new List<Piece> ();

		XmlTextReader myXmlTextReader = new XmlTextReader(path);
		
		while (myXmlTextReader.Read())
		{
			Piece piece = new Piece ();

			if(myXmlTextReader.IsStartElement()){
				if (myXmlTextReader.Name == "piece")
				{
					piece.id = int.Parse(myXmlTextReader.GetAttribute("id"));
					piece.name = myXmlTextReader.GetAttribute("name");
					piece.rotY = float.Parse(myXmlTextReader.GetAttribute("rotY"));
					piece.mur = myXmlTextReader.GetAttribute("mur");

					piece.prefab = Resources.Load<GameObject>("Pieces/" + piece.name) as GameObject;

					listPiecesTmp.Add (piece);
				}
			}
		}
		
		myXmlTextReader.Close();
		listPieces = new Piece[listPiecesTmp.Count];
		listPieces = listPiecesTmp.ToArray ();

		//Show all the pieces in the console
		foreach (Piece p in listPieces) {
			Debug.Log(p.ToString());
		}
	}

	public void LoadLevel(int idLevel){
		CheckIfFolderDocsExist ();

		string path = SearchPath (folderLevels, "levels");
		path = path.Replace ("\\", "/");
		
		Level level = new Level();
		bool levelFind = false;
		
		XmlTextReader myXmlTextReader = new XmlTextReader(path);
		
		while (myXmlTextReader.Read())
		{
			if(myXmlTextReader.IsStartElement()){
				if (myXmlTextReader.Name == "level" && int.Parse(myXmlTextReader.GetAttribute("id")) == idLevel)
				{
					//Level name & id //////////////////////////////////////
					level.id = int.Parse(myXmlTextReader.GetAttribute("id"));
					level.name = myXmlTextReader.GetAttribute("name");

					//PosBille at the beginning /////////////////////////////
					myXmlTextReader.ReadToFollowing("posBille");
					float x = float.Parse(myXmlTextReader.GetAttribute("x"));
					float y = float.Parse(myXmlTextReader.GetAttribute("y"));
					float z = float.Parse(myXmlTextReader.GetAttribute("z"));
					level.posBille.Set(x,y,z);

					//PosExit in the maze /////////////////////////////////
					myXmlTextReader.ReadToFollowing("posExit");
					x = float.Parse(myXmlTextReader.GetAttribute("x"));
					y = float.Parse(myXmlTextReader.GetAttribute("y"));
					z = float.Parse(myXmlTextReader.GetAttribute("z"));
					level.posExit.Set(x,y,z);

					//Width, Height and content of the maze /////////////////
					myXmlTextReader.ReadToFollowing("labyrinthe");
					level.width = int.Parse(myXmlTextReader.GetAttribute("width"));
					level.height = int.Parse(myXmlTextReader.GetAttribute("height"));
	
					string lab = myXmlTextReader.ReadElementContentAsString();
					level.labyrinthe = lab.Split(Level.charSeparator[0]);

					//Time medals ///////////////////////////////////////////
					myXmlTextReader.ReadToFollowing("time"); 
					level.timeGold = float.Parse(myXmlTextReader.GetAttribute("gold"));
					level.timeSilver = float.Parse(myXmlTextReader.GetAttribute("silver"));
					level.timeBronze = float.Parse(myXmlTextReader.GetAttribute("bronze"));

					levelFind = true;
				}
			}

			if(levelFind)
				break;
		}
		
		myXmlTextReader.Close();

		if(level.id == idLevel){
			currentLevel = level;
			Debug.Log (currentLevel.ToString ());
		}else{
			Debug.LogError("Impossible de charger le Labyrinthe n°" + idLevel);
			return;
		}
	}

	private string SearchPath(string folder, string contains){
		string [] path;

		path = Directory.GetFiles (Application.dataPath + folderDocs + folder);
		
		foreach(string str in path){
			if(str.EndsWith(".xml")){
				if(str.Contains(contains)){
					return str;
				}
			}
		}

		return "";
	}

	private void GenerateLabyrinthe(){
		if (currentLevel == null) {
			Debug.LogError("Le level courant n'a pas été chargé, impossible de générer le labyrinthe.");
			return;
		}

		if (currentLevel.labyrinthe.Length != currentLevel.width * currentLevel.height) {
			Debug.LogError("Le level courant ne contient pas autant de pièces que sa taille. Nb pièces : " + currentLevel.labyrinthe.Length + " Taille X : " + currentLevel.width + " Taille Z : " + currentLevel.height);
			return;
		}
		//Add walls
		GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//wall.transform.SetParent (plateau.transform);
		wall.transform.localScale = new Vector3(currentLevel.width * sizePiece, 3f, 1f);
		wall.transform.position = new Vector3(0f, 0f,(currentLevel.width / 2f) * sizePiece + 0.5f);
		for (int index = 1; index < 4; index++) {
			GameObject w = Instantiate(wall) as GameObject;
			w.transform.RotateAround(Vector3.zero, new Vector3(0f,1f,0f), index * 90f);
		}

		//Add pieces
		int i;
		float posX = - (currentLevel.width / 2) * sizePiece;
		float posZ = (currentLevel.height / 2) * sizePiece;
		for (int z = 0; z < currentLevel.height; z++) {
			for(int x = 0 ; x < currentLevel.width ; x++) {
				i = int.Parse(currentLevel.labyrinthe[z * currentLevel.width + x]);
				GameObject go = GameObject.Instantiate(listPieces[i].prefab, new Vector3(posX, 0f, posZ), Quaternion.Euler(new Vector3(0f, listPieces[i].rotY, 0f))) as GameObject;
				go.transform.SetParent(plateau.transform);

				posX += sizePiece;
			}
			posZ -= sizePiece;
			posX = -(currentLevel.width / 2) * sizePiece;
		}
		//Add exit
		GameObject go_exit = GameObject.Instantiate (exit, currentLevel.posExit, Quaternion.identity) as GameObject;
		go_exit.transform.SetParent(plateau.transform);

		//Add sphere
		GameObject.Instantiate (bille, currentLevel.posBille, Quaternion.identity);
	}

	public static void CheckIfFolderDocsExist(){
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			bool folderExist = System.IO.Directory.Exists (Application.dataPath + folderDocs);
			
			if (!folderExist) {
				System.IO.Directory.CreateDirectory (Application.dataPath + folderDocs);
				return;
			}
		} else {
			Debug.LogError("Platform non prise en charge " + Application.platform);
			return;
		}
	}


}
