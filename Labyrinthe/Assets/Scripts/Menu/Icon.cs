using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Icon : MonoBehaviour {

	public int id;
	public Sprite img;
	public string name;

	public Image imgLevel;
	public Text nameLevel;

	void Start(){

		Apply();
	}
	
	public void Init(int id, Sprite img, string name){
		this.id = id;
		this.img = img;
		this.name = name;
	}

	public void Apply(){
		imgLevel.sprite = img;
		nameLevel.text = name;
	}


	public void OnClick(){
		Debug.Log("OnClick = Id : " + id + " name : " + name);
		LevelManager.setLevelToLoad(id);
	}


	public override string ToString(){
		return "Icon : Id = " + id + " name : " + name;

	}
}
