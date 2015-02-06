using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class LabyrintheManager : MonoBehaviour {

	private string folderDocs = "/Documents";
	private string folderLevels = "/Levels";
	private string folderPieces = "/Pieces";
	private string folderSave = "/Save";

	private List<Piece> listPieces;
	private Level currentLevel;

	void Awake(){
		DontDestroyOnLoad (gameObject);

		listPieces = new List<Piece> ();
		//LoadPieces ();

		LoadLevel (2);
		
	}

	public void LoadPieces(){
		CheckIfFolderDocsExist ();

		string path = SearchPath (folderPieces, "pieces");
		if (path == "") {
			Debug.LogError("Impossible d'ouvrir le fichier pieces.xml");
			return;
		}

		path = path.Replace ("\\", "/");

		Piece piece = new Piece ();

		XmlTextReader myXmlTextReader = new XmlTextReader(path);
		
		while (myXmlTextReader.Read())
		{
			if(myXmlTextReader.IsStartElement()){
				if (myXmlTextReader.Name == "piece")
				{
					piece.id = int.Parse(myXmlTextReader.GetAttribute("id"));
					piece.name = myXmlTextReader.GetAttribute("name");
					piece.rotY = float.Parse(myXmlTextReader.GetAttribute("rotY"));
					piece.mur = myXmlTextReader.GetAttribute("mur");

					listPieces.Add(piece);
				}
			}
		}
		
		myXmlTextReader.Close();

		//Show all the pieces in the console
		/*foreach (Piece p in listPieces) {
			Debug.Log(p.id + " " + p.name);
		}*/
	}

	public void LoadLevel(int idLevel){
		CheckIfFolderDocsExist ();

		string path = SearchPath (folderLevels, "levels");
		path = path.Replace ("\\", "/");
		
		Level level = new Level();
		
		XmlTextReader myXmlTextReader = new XmlTextReader(path);
		
		while (myXmlTextReader.Read())
		{
			if(myXmlTextReader.IsStartElement()){
				if (myXmlTextReader.Name == "level")
				{
					int id = int.Parse(myXmlTextReader.GetAttribute("id"));
					if(id != idLevel)
						continue;

					level.id = id;
					level.name = myXmlTextReader.GetAttribute("name");
				}else if (myXmlTextReader.Name == "posBille")
				{
					float x = float.Parse(myXmlTextReader.GetAttribute("x"));
					float y = float.Parse(myXmlTextReader.GetAttribute("y"));
					float z = float.Parse(myXmlTextReader.GetAttribute("z"));

					level.posBille.Set(x,y,z);
				}else if (myXmlTextReader.Name == "labyrinthe")
				{
					level.width = int.Parse(myXmlTextReader.GetAttribute("width"));
					level.height = int.Parse(myXmlTextReader.GetAttribute("height"));

					string lab = myXmlTextReader.ReadString();
					level.labyrinthe = lab.Split(Level.charSeparator[0]);
				}else if (myXmlTextReader.Name == "time")
				{
					level.timeGold = float.Parse(myXmlTextReader.GetAttribute("gold"));
					level.timeSilver = float.Parse(myXmlTextReader.GetAttribute("silver"));
					level.timeBronze = float.Parse(myXmlTextReader.GetAttribute("bronze"));
				}
			}
		}
		
		myXmlTextReader.Close();

		if(level.id == idLevel)
			currentLevel = level;
		else
			Debug.LogError("Impossible de charger le Labyrinthe n°" + idLevel);
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

	public void CheckIfFolderDocsExist(){
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
