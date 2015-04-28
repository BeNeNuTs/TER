using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	public int id = 0;
	public Level.LevelType type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		id = collider.gameObject.GetComponent<CubeLevel> ().id;
		type = collider.gameObject.GetComponent<CubeLevel> ().type;
		Debug.Log ("ID : " + id);
		//LevelManager.setLevelToLoad(collider.gameObject.GetComponent<CubeLevel>().id, collider.gameObject.GetComponent<CubeLevel>().type );


	}
}
