﻿using UnityEngine;
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

	private Level currentLevel;
	
	public GameObject bille;
	public GameObject exit;

	public Maze maze;

	void Start(){
		currentLevel = null;
		//GenerateLabyrinthe(1);
	}

	public static XmlTextReader GetLevelXML(){
		CheckIfFolderDocsExist ();
		
		string path = SearchPath (folderLevels, "levels");
		path = path.Replace ("\\", "/");

		return new XmlTextReader(path);
	}

	public static XmlTextReader GetSavedLevelXML(){
		CheckIfFolderDocsExist ();
		
		string path = SearchPath (folderSave, "savedLevels");
		path = path.Replace ("\\", "/");

		return new XmlTextReader(path);
	}

	public static Level LoadLevel(XmlTextReader myXmlTextReader, int idLevel){
		Level level = new Level();
		bool levelFind = false;
		
		while (myXmlTextReader.Read())
		{
			if(myXmlTextReader.IsStartElement()){
				if (myXmlTextReader.Name == "level" && int.Parse(myXmlTextReader.GetAttribute("id")) == idLevel)
				{
					//Level name & id //////////////////////////////////////
					level.id = int.Parse(myXmlTextReader.GetAttribute("id"));
					level.name = myXmlTextReader.GetAttribute("name");
					level.img = myXmlTextReader.GetAttribute("img");

					//PosBille at the beginning /////////////////////////////
					myXmlTextReader.ReadToFollowing("posBille");
					int x = int.Parse(myXmlTextReader.GetAttribute("x"));
					int y = int.Parse(myXmlTextReader.GetAttribute("y"));
					level.posBille = new IntVector2(x,y);

					//PosExit in the maze /////////////////////////////////
					myXmlTextReader.ReadToFollowing("posExit");
					x = int.Parse(myXmlTextReader.GetAttribute("x"));
					y = int.Parse(myXmlTextReader.GetAttribute("y"));
					level.posExit = new IntVector2(x,y);

					//Width, Height and content of the maze /////////////////
					myXmlTextReader.ReadToFollowing("labyrinthe");
					level.width = int.Parse(myXmlTextReader.GetAttribute("width"));
					level.height = int.Parse(myXmlTextReader.GetAttribute("height"));

					myXmlTextReader.ReadToFollowing("lines");
					string lines = myXmlTextReader.ReadElementContentAsString();
					level.lines = lines.Split(Level.charSeparator[0]);

					myXmlTextReader.ReadToFollowing("columns");
					string columns = myXmlTextReader.ReadElementContentAsString();
					level.columns = columns.Split(Level.charSeparator[0]);

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
			return level;
		}else{
			Debug.LogError("Impossible de charger le Labyrinthe n°" + idLevel);
			return null;
		}
	}

	public static string SearchPath(string folder, string file){
		string [] path;

		path = Directory.GetFiles (Application.dataPath + folderDocs + folder);
		
		foreach(string str in path){
			if(str.EndsWith(".xml")){
				if(str.Contains(file)){
					return str;
				}
			}
		}

		return "";
	}

	public void GenerateLabyrinthe(int level, Level.LevelType levelType){
		if(levelType == Level.LevelType.Level)
			currentLevel = LoadLevel(LabyrintheManager.GetLevelXML(), level);
		else
			currentLevel = LoadLevel(LabyrintheManager.GetSavedLevelXML(), level);

		if(currentLevel == null)
			return;

		currentLevel.levelType = levelType;

		if (currentLevel.lines.Length != currentLevel.width || currentLevel.columns.Length != currentLevel.height) {
			Debug.LogError("Le level courant ne contient pas autant de pièces que sa taille. Lignes : " + currentLevel.lines.Length + " Colonnes : " + currentLevel.columns.Length + " Taille X : " + currentLevel.width + " Taille Z : " + currentLevel.height);
			return;
		}
		if (currentLevel.posBille.x > currentLevel.width - 1 || currentLevel.posBille.z > currentLevel.height - 1) {
			Debug.LogError("Position de départ de la bille hors du labyrinthe. Position bille : " + currentLevel.posBille.ToString() + " Taille X : " + currentLevel.width + " Taille Z : " + currentLevel.height);
			return;
		}
		if (currentLevel.posExit.x > currentLevel.width - 1 || currentLevel.posExit.z > currentLevel.height - 1) {
			Debug.LogError("Position de la sortie est hors du labyrinthe. Position sortie : " + currentLevel.posExit.ToString() + " Taille X : " + currentLevel.width + " Taille Z : " + currentLevel.height);
			return;
		}

		GameController.currentLevel = currentLevel;
		StartCoroutine(maze.GenerateLevel(currentLevel));
		bille = Instantiate(bille) as GameObject;
		bille.transform.position = new Vector3(maze.GetCell(currentLevel.posBille).transform.position.x, 0.4f, maze.GetCell(currentLevel.posBille).transform.position.z);
		bille.transform.parent = maze.transform;

		exit = Instantiate(exit) as GameObject;
		exit.transform.position = new Vector3(maze.GetCell(currentLevel.posExit).transform.position.x, 0f, maze.GetCell(currentLevel.posExit).transform.position.z);
		exit.transform.parent = maze.transform;
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

	public int LevelId {
		get {
			return currentLevel.id;
		}
	}


}
