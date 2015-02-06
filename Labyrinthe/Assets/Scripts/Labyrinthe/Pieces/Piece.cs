using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece {

	public int id;
	public string name;
	public float rotY;
	public string mur;

	public static char [] charSeparator = new char[]{'-'};

	public Piece(){
		this.id = 0;
		this.name = "";
		this.rotY = 0f;
		this.mur = "";
	}

	public Piece(int id, string name, float rotY, string mur){
		this.id = id;
		this.name = name;
		this.rotY = rotY;
		this.mur = mur;
	}
}
