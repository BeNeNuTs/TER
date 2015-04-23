using UnityEngine;
using System.Collections;

public class CubeLevel : MonoBehaviour {

	public int id;
	public Level.LevelType type;

	// Use this for initialization
	void Start () 
	{
		Texture myTexture = Resources.Load<Texture> ("img/" + id) as Texture;
		Debug.Log (myTexture);
		Material myMaterial = gameObject.GetComponent<MeshRenderer> ().material;
		Debug.Log (myMaterial);

		myMaterial.SetTexture (0, myTexture);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
