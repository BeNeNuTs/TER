using UnityEngine;
using System.Collections;

public class Level {

	public enum LevelType { Level, SavedLevel };

	public int id;
	public string name;
	public string img;
	public IntVector2 posBille;
	public IntVector2 posExit;

	public int width;
	public int height;

	public string [] lines;
	public string [] columns;

	public float timeGold;
	public float timeSilver;
	public float timeBronze;
	public LevelType levelType;

	public static char [] charSeparator = new char[]{'|'};

	public Level(){
		this.id = 0;
		this.name = "";
		this.img = "";
		this.posBille = new IntVector2();
		this.posExit = new IntVector2();
		this.width = 0;
		this.height = 0;
		this.lines = null;
		this.columns = null;
		this.timeGold = 0f;
		this.timeSilver = 0f;
		this.timeBronze = 0f;
		this.levelType = LevelType.Level;
	}
	
	public override string ToString(){
		return "Id : " + id + " name : " + name + " posBille : " + posBille + " posExit : " + posExit + " Width x Height : " + width + " x " + height;
	}

}
