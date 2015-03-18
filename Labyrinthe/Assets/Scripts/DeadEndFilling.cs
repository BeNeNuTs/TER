using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DeadEndFilling : MonoBehaviour {

	public float generationStepDelay;
	private Maze m;
	public int startX, startZ;
	public int endX, endZ;
	private int[,] passageNumber;
	private List<IntVector2> deadEnds;

	//Compte le nombre de murs dans une cellule
	private int countPassage(int x, int z){
		int res = 0;
		for (int i = 0; i != MazeDirections.Count; ++i) {
			if (m.GetCell (new IntVector2 (x, z)).GetEdge ((MazeDirection) i).name.Contains ("MazePassage"))
				++res;
		}
		return res;
	}

	//Initialise les variables de la classe
	private void init(Maze maze){
		m = maze;
		passageNumber = new int[m.size.x, m.size.z];
		deadEnds = new List<IntVector2>();
	}

	// Remplit les trous dans le labyrinthe à partir des coordonnées indiquées
	private void fillMaze(IntVector2 coords){
		// On vérifie qu'on est pas au début ou à la fin
		if ((coords.x != startX || coords.z != startZ) && (coords.x != endX || coords.z != endZ)) {
			passageNumber[coords.x,coords.z] -= 1;
			drawBadPath(coords.x,coords.z, 1f, 0f, 0f);

			// Pour une case remplie, on étend aux couloirs
			foreach(MazeDirection md in Enum.GetValues(typeof(MazeDirection))){
				if(m.GetCell(coords).GetEdge(md).name.Contains("MazePassage")){
					IntVector2 next = m.GetCell(coords).GetEdge(md).otherCell.coordinates;
					passageNumber[next.x, next.z] -= 1;
					if(passageNumber[next.x, next.z] == 1)
						fillMaze(next);
				}
			}
		}
	}

	private void drawBadPath(int x, int z, float r, float g, float b){
		m.GetCell (new IntVector2(x,z)).gameObject.GetComponentInChildren<MeshRenderer> ().materials [0].color = new Color (r, g, b);
	}

	// Algorithme du dead-end filling
	public void deadEndFilling(Maze maze){
		// Initialisation des variables
		init (maze);

		// Initialiser les murs et trouver les dead-ends
		for (int i = 0; i != m.size.x; ++i) {
			for(int j = 0; j != m.size.z; ++j){
				passageNumber[i,j] = countPassage(i,j);
				if(passageNumber[i,j] == 1){
					drawBadPath(i, j, 0f , 0f, 0f);
					deadEnds.Add(new IntVector2(i,j));
				}
			}
		}

		// Remplir les dead-ends
		foreach (IntVector2 de in deadEnds) {
			fillMaze(de);
		}

		drawBadPath (startX, startZ, 0f, 1f, 0f);
		drawBadPath (endX, endZ, 0f, 0f, 1f);
	}
}
