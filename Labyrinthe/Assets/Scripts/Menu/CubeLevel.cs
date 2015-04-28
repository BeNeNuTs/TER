using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CubeLevel : MonoBehaviour {

	public int id;
	public Level.LevelType type;
	public int nbStars = 0;
	public GameObject[] stars;
	public Text time;
	public Text nameText;


	// Use this for initialization
	void Start () 
	{
		Texture myTexture = Resources.Load<Texture> ("img/" + id) as Texture;
		Debug.Log (myTexture);
		Material myMaterial = gameObject.GetComponent<MeshRenderer> ().material;
		Debug.Log (myMaterial);

		myMaterial.SetTexture (0, myTexture);

		for (int cptStar = 0; cptStar < nbStars; cptStar++) 
		{
			stars[cptStar].GetComponent<Image>().enabled = true;
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
