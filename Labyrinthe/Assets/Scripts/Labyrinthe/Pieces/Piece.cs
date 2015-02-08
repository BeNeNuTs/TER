using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece {

	public int id;
	public string name;
	public float rotY;
	public string mur;
	public GameObject prefab;

	public static char [] charSeparator = new char[]{'-'};

	public Piece(){
		this.id = 0;
		this.name = "";
		this.rotY = 0f;
		this.mur = "";
		this.prefab = null;
	}

	public Piece(int id, string name, float rotY, string mur, GameObject prefab){
		this.id = id;
		this.name = name;
		this.rotY = rotY;
		this.mur = mur;
		this.prefab = prefab;
	}

	public string ToString() {
		return "Id : " + id + " name : " + name + " rotY : " + rotY;
	}

	public static bool isCompatible(Piece p1, Piece p2){
		return true;
	}
}
