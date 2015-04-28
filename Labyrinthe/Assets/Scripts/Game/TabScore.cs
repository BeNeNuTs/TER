using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class TabScore : MonoBehaviour {

	public Text scoreText;
	public Text timeText;
	public GameObject highScore;

	public GameObject [] fillStars;
	public float delayStar = 0.7f;

	private int starPoint = 10000;
	private AudioSource sound;

	/** Récupère le composant AudioSource du gameObject pour pouvoir lancer le sons pour les étoiles */
	void Start(){
		sound = GetComponent<AudioSource>();
	}

	/** Génère le score, le temps et le nombre d'étoile du joueur à la fin d'une partie */
	public void GenerateScore(Level currentLevel, float time){
		int nbStars = 0;
		float beatTime = 0f;

		if(time < currentLevel.timeGold){
			beatTime = currentLevel.timeGold;
			nbStars = 3;
		}else if(time < currentLevel.timeSilver){
			beatTime = currentLevel.timeSilver;
			nbStars = 2;
		}else if(time < currentLevel.timeBronze){
			beatTime = currentLevel.timeBronze;
			nbStars = 1;
		}

		//Afficher les étoiles
		StartCoroutine(ShowStar(nbStars, delayStar));
		int score = 0;
		if(nbStars == 0){
			score = starPoint - Mathf.FloorToInt(Mathf.Abs(time - currentLevel.timeBronze) * 1000);
			if(score < 0)
				score = 0;
		}else if(nbStars > 0){
			score = nbStars * starPoint + Mathf.FloorToInt(Mathf.Abs(time - beatTime) * 1000);
		}
		scoreText.text += score;
		timeText.text += time;


		//Si record battu
		if(currentLevel.score == null || score > currentLevel.score){
			//Afficher "Nouveau meilleur score !!!"
			iTween.ScaleTo(highScore, iTween.Hash("scale", new Vector3(1f,1f,1f), "delay", delayStar, "time", 1f, "easetype", iTween.EaseType.easeOutElastic));

			//Sauvegarder le nouveau meilleur score
			SaveHighScore(score, time, nbStars);
		}

	}

	/** Sauvegarde les meilleurs scores dans le XML pour le niveau joué */
	private void SaveHighScore(int score, float time, int nbStars){
		XmlTextReader myXmlTextReader;
		if(GameController.currentLevel.levelType == Level.LevelType.Level){
			myXmlTextReader = LabyrintheManager.GetLevelXML();
		}else{
			myXmlTextReader = LabyrintheManager.GetSavedLevelXML();
		}
		
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(myXmlTextReader);
		
		myXmlTextReader.Close();
		
		XmlNodeList levelNodes = xdoc.GetElementsByTagName("level");
		
		for (int i = 0; i < levelNodes.Count; i++)
		{
			if (levelNodes[i].Attributes["id"] != null)
			{
				if (GameController.currentLevel.id.ToString() == levelNodes[i].Attributes["id"].InnerText)
				{    
					levelNodes[i].Attributes["score"].InnerText = score.ToString();
					levelNodes[i].Attributes["time"].InnerText = time.ToString();
					levelNodes[i].Attributes["stars"].InnerText = nbStars.ToString();
					if(GameController.currentLevel.levelType == Level.LevelType.Level){
						xdoc.Save(Application.dataPath + LabyrintheManager.folderDocs + LabyrintheManager.folderLevels + "/levels.xml");
					}else{
						xdoc.Save(Application.dataPath + LabyrintheManager.folderDocs + LabyrintheManager.folderSave + "/savedLevels.xml");
					}
					return;
				}
			}
		}
		
		Debug.LogError("Level to update high score doesn't find.");
	}

	/** Permet d'afficher les étoiles avec une animation */
	IEnumerator ShowStar(int nbStars, float delay){
		yield return new WaitForSeconds(delay);

		for(int i = 0 ; i < nbStars ; i++){
			iTween.ScaleTo(fillStars[i], iTween.Hash("scale", new Vector3(1.2f,1.2f,1f), "time", 1f, "easetype", iTween.EaseType.easeOutElastic));
			sound.Play();
			yield return new WaitForSeconds(delay);
		}
	}
}
