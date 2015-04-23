using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	public int id = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		id = collider.gameObject.GetComponent<CubeLevel>().id;
		Debug.Log ("ID : " + id);
		//LevelManager.setLevelToLoad(collider.gameObject.GetComponent<CubeLevel>().id, collider.gameObject.GetComponent<CubeLevel>().type );


	}
}
