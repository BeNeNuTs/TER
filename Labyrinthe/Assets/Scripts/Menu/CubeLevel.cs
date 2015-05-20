using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/** Classe permettant de définir les cubes des différents niveaux dans le carrousel */
public class CubeLevel : MonoBehaviour {

	public int id;
	public Level.LevelType type;
	public int nbStars = 0;
	public GameObject[] stars;
	public Text time;
	public Text nameText;


	/** Initiliase les étoiles du cube */
	void Start () 
	{
		for (int cptStar = 0; cptStar < nbStars; cptStar++) 
		{
			stars[cptStar].GetComponent<Image>().enabled = true;
		}
	}

	/** Permet d'ajouter l'image du labyrinthe sur le cube */
	public void setImage(){
		if (type == Level.LevelType.Level) {
			Texture myTexture = Resources.Load<Texture> ("img/levels/" + id) as Texture;
			Material myMaterial = gameObject.GetComponent<MeshRenderer> ().material;
			myMaterial.SetTexture (0, myTexture);
		} else {
			StartCoroutine(LoadImage());
		}
	}

	private IEnumerator LoadImage() {
		
		string imagePath = "file://" + Application.dataPath + "/Documents/Save/img/" + id + ".png";
		
		WWW www = new WWW(imagePath);
		
		yield return www;

		if(www.texture != null){
			Material myMaterial = gameObject.GetComponent<MeshRenderer> ().material;
			myMaterial.SetTexture (0, www.texture);
		}else{
			Debug.LogError("Impossible de charger la texture du cube : " + id);
		}
	}
}
