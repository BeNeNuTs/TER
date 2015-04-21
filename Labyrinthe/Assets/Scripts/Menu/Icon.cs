using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Icon : MonoBehaviour {

	public int id;
	public Level.LevelType levelType;

	private Image imgIcon;
	private Text levelTitle;

	void Start(){
		imgIcon = transform.FindChild("Image").GetComponent<Image>();
		levelTitle = transform.FindChild("NameText").GetComponent<Text>();

		if(imgIcon == null){
			Debug.LogError("Impossible de trouver Image dans child");
			return;
		}
		if(levelTitle == null){
			Debug.LogError("Impossible de trouver NameText dans child");
			return;
		}

		Init();
	}

	public void Init(){
		if(id < 0){
			Debug.LogError("id < 0, impossible de charger l'icone avec l'id = "+id);
			return;
		}

		Level level;

		if(levelType == Level.LevelType.Level){
			level = LabyrintheManager.LoadLevel(LabyrintheManager.GetLevelXML(), id);
		}else{
			level = LabyrintheManager.LoadLevel(LabyrintheManager.GetSavedLevelXML(), id);
		}

		imgIcon.sprite = Resources.Load<Sprite>(level.img) as Sprite;
		levelTitle.text = level.name;
	}
	
	public void OnClick(){
		LevelManager.setLevelToLoad(id, levelType);
	}


	public override string ToString(){
		return "Icon : Id = " + id + " name : " + name;

	}
}
