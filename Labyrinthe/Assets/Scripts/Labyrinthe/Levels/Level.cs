using UnityEngine;
using System.Collections;

public class Level {

	public int id;
	public string name;
	public Vector3 posBille;

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
		this.posBille = Vector3.zero;
		this.width = 0;
		this.height = 0;
		this.labyrinthe = null;
		this.timeGold = 0f;
		this.timeSilver = 0f;
		this.timeBronze = 0f;
	}

	public Level(int id, string name, Vector3 posBille, int width, int height, string labyrinthe, float timeGold, float timeSilver, float timeBronze){
		this.id = id;
		this.name = name;
		this.posBille = posBille;
		this.width = width;
		this.height = height;
		this.labyrinthe = labyrinthe.Split (Level.charSeparator[0]);
		this.timeGold = timeGold;
		this.timeSilver = timeSilver;
		this.timeBronze = timeBronze;
	}

}
