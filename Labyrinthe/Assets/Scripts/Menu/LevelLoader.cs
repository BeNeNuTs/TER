using UnityEngine;
using System.Collections;

/** Classe permettant de définir le niveau à charger dans le carrousel */
public class LevelLoader : MonoBehaviour {

	public int id = 0;
	public Level.LevelType type;
	public GameObject cube;

	/** Détecte lorsqu'un GameObject rentre dans le trigger de l'objet
	 *  et récupère son type et son id 
	 */
	void OnTriggerEnter(Collider collider)
	{
		id = collider.gameObject.GetComponent<CubeLevel> ().id;
		type = collider.gameObject.GetComponent<CubeLevel> ().type;
		cube = collider.gameObject;
	}
}
