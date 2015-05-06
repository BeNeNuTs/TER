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
		Texture myTexture;
		if (type == Level.LevelType.Level) {
			myTexture = Resources.Load<Texture> ("img/levels/" + id) as Texture;
		} else {
			myTexture = Resources.Load<Texture> ("img/savedLevels/" + id) as Texture;
		}

		Debug.Log (myTexture);
		Material myMaterial = gameObject.GetComponent<MeshRenderer> ().material;
		Debug.Log (myMaterial);
		
		myMaterial.SetTexture (0, myTexture);
	}
}
