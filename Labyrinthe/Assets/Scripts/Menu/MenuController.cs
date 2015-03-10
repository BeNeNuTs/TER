using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class MenuController : MonoBehaviour {

	public GameObject menu;
	public GameObject levels;
	public GameObject options;

	public GameObject [] tabIcon;

	public float timeTransition = 2f;

	public struct IconStruct
	{
		public int id;
		public Sprite img;
		public string name;
	};

	void Awake(){
		Debug.Log("Screen : width = " + Screen.width + " height = " + Screen.height);

		//Désactiver la vue des levels et la mettre à droite
		levels.SetActive(false);
		levels.transform.position = new Vector3(Screen.width * 2,Mathf.Floor(Screen.height/2),0);

		//Désactiver la vue des options et la mettre à gauche
		options.SetActive(false);
		options.transform.position = new Vector3(-Screen.width * 2,Mathf.Floor(Screen.height/2),0);


		//Load level icons
		CreateAllLevelIcons(LabyrintheManager.GetLevelXML());
	}

	public void CreateAllLevelIcons(XmlTextReader myXmlTextReader){
		ArrayList levelsIcon = new ArrayList();
		
		while (myXmlTextReader.Read())
		{
			if(myXmlTextReader.IsStartElement()){
				if (myXmlTextReader.Name == "level")
				{
					//Level id //////////////////////////////////////
					IconStruct icon;
					icon.id = int.Parse(myXmlTextReader.GetAttribute("id"));
					string imgPath = myXmlTextReader.GetAttribute("img");
					icon.name = myXmlTextReader.GetAttribute("name");

					icon.img = Resources.Load<Sprite>(imgPath) as Sprite;

					levelsIcon.Add(icon);
				}
			}
		}
		
		myXmlTextReader.Close();

		for(int i = 0 ; i < levelsIcon.Count ; i++)	
			Debug.Log(levelsIcon[i]);

		if(levelsIcon.Count != tabIcon.Length){
			Debug.LogError("Nombre de levels != du nombre d'icon dans le menu : nbLevel : " + levelsIcon.Count + " nbIcon : " + tabIcon.Length);
			return;
		}

		/*for(int i = 0 ; i < tabIcon.Length ; i++){
			tabIcon[i].GetComponent<Icon>().id = levelsIcon[i];
		}*/
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

	public void Quit(){
		Debug.Log("Quit");

		if(Application.isEditor)
			Debug.Break();
		else
			Application.Quit();
	}
}
