using UnityEngine;
using System.Collections;

public class DeadEndFilling : MonoBehaviour {

	private Maze m;
	public int startX, startZ;
	public int endX, endZ;
	private bool[,] correctPath;
	private int[,] passageNumber;

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
		correctPath = new bool[m.size.x, m.size.z];
		passageNumber = new int[m.size.x, m.size.z];
	}

	// Remplit les trous dans le labyrinthe à partir des coordonnées indiquées
	private void fillMaze(int x, int z){
		// On vérifie qu'on est pas au début ou à la fin
		if ((x != startX && z != startZ) && (x != endX && z != endZ)) {
			correctPath[x,z] = false;
			drawCellBlack(x,z);


		}
	}

	private void drawCellBlack(int x, int z){

	}

	// Algorithme du dead-end filling
	public void deadEndFilling(Maze maze){
		// Initialisation des variables
		init (maze);

		// Initialiser les murs et trouver les dead-ends
		for (int i = 0; i != m.size.x; ++i) {
			for(int j = 0; j != m.size.z; ++j){
				passageNumber[i,j] = countPassage(i,j);
			}
		}

		// Remplir les dead-ends
		for (int i = 0; i != m.size.x; ++i) {
			for(int j = 0; j != m.size.z; ++j){
				if(passageNumber[i,j] == 1)
					fillMaze (i,j);
				else if (passageNumber[i,j] == 0)
					drawCellBlack(i,j);
			}
		}
	}
}
