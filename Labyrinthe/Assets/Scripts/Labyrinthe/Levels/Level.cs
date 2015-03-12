using UnityEngine;
using System.Collections;

public class Level {

	public int id;
	public string name;
	public string img;
	public IntVector2 posBille;
	public IntVector2 posExit;

	public int width;
	public int height;

	public string [] labyrinthe;

	public float timeGold;
	public float timeSilver;
	public float timeBronze;

	public static char [] charSeparator = new char[]{'-'};

	public Level(){
		this.id = 0;
		this.name = "";
		this.img = "";
		this.posBille = new IntVector2();
		this.posExit = new IntVector2();
		this.width = 0;
		this.height = 0;
		this.labyrinthe = null;
		this.timeGold = 0f;
		this.timeSilver = 0f;
		this.timeBronze = 0f;
	}

	public Level(int id, string name, string img, IntVector2 posBille, IntVector2 posExit, int width, int height, string labyrinthe, float timeGold, float timeSilver, float timeBronze){
		this.id = id;
		this.name = name;
		this.img = img;
		this.posBille = posBille;
		this.posExit = posExit;
		this.width = width;
		this.height = height;
		this.labyrinthe = labyrinthe.Split (Level.charSeparator[0]);
		this.timeGold = timeGold;
		this.timeSilver = timeSilver;
		this.timeBronze = timeBronze;
	}
	
	public string ToString(){
		return "Id : " + id + " name : " + name + " posBille : " + posBille + " posExit : " + posExit + " Width x Height : " + width + " x " + height;
	}

}
