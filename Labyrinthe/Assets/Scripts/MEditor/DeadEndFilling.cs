using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** Classe définissant l'algorithme de dead-end filling */
public class DeadEndFilling : MonoBehaviour {

	public float generationStepDelay;

	private int[,] passageNumber;
	private List<IntVector2> deadEnds;
	private Maze m;
	private IntVector2 start;
	private IntVector2 end;

	/** Compte le nombre de passages dans une cellule */
	private int countPassage(int x, int z){
		int res = 0;
		for (int i = 0; i != MazeDirections.Count; ++i) {
			if (m.GetCell (new IntVector2 (x, z)).GetEdge ((MazeDirection) i).GetType() == typeof(MazePassage))
				++res;
		}
		return res;
	}

	/** Initialise les variables de la classe */
	private void init(Maze maze, IntVector2 start, IntVector2 end){
		m = maze;
		this.start = start;
		this.end = end;
		passageNumber = new int[m.size.x, m.size.z];
		deadEnds = new List<IntVector2>();
	}

	public void clear(){
		if(m == null){
			return;
		}

		for (int i = 0; i != m.size.x; ++i) {
			for(int j = 0; j != m.size.z; ++j){
				drawBadPath(i, j, Color.white);
			}
		}
	}

	/** Remplit les trous dans le labyrinthe à partir des coordonnées indiquées */
	private void fillMaze(IntVector2 coords){
		// On vérifie qu'on est pas au début ou à la fin
		if ((coords.x != start.x || coords.z != start.z) && (coords.x != end.x || coords.z != end.z)) {
			passageNumber[coords.x,coords.z] -= 1;
			drawBadPath(coords.x,coords.z, new Color(1f,0f,0f));

			// Pour une case remplie, on étend aux couloirs
			foreach(MazeDirection md in Enum.GetValues(typeof(MazeDirection))){
				if(m.GetCell(coords).GetEdge(md).GetType() == typeof(MazePassage)){
					IntVector2 next = m.GetCell(coords).GetEdge(md).otherCell.coordinates;
					passageNumber[next.x, next.z] -= 1;
					if(passageNumber[next.x, next.z] == 1)
						fillMaze(next);
				}
			}
		}
	}

	/** Colorie avec la couleur c, la cellule aux coordonnées passées en paramètres */
	private void drawBadPath(int x, int z, Color c){
		m.GetCell (new IntVector2(x,z)).gameObject.GetComponentInChildren<MeshRenderer> ().materials [0].color = c;
	}


	/** Algorithme du dead-end filling */
	public void deadEndFilling(Maze maze, IntVector2 start, IntVector2 end){
		// Initialisation des variables
		init (maze, start, end);

		// Initialiser les murs et trouver les dead-ends
		for (int i = 0; i != m.size.x; ++i) {
			for(int j = 0; j != m.size.z; ++j){
				passageNumber[i,j] = countPassage(i,j);
				if(passageNumber[i,j] == 1){
					drawBadPath(i, j, new Color(0f,0f,0f));
					deadEnds.Add(new IntVector2(i,j));
				}
			}
		}

		// Remplir les dead-ends
		foreach (IntVector2 de in deadEnds) {
			fillMaze(de);
		}

		drawBadPath (start.x, start.z, new Color(0f,1f,0f));
		drawBadPath (end.x, end.z, new Color(0f,0f,1f));
	}
}
