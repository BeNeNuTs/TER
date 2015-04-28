using UnityEngine;
using System.Collections;

/** Classe permettant de définir une cellule */
public class MazeCell : MonoBehaviour {

	public IntVector2 coordinates;

	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];
	private int initializedEdgeCount;

	/** Retourne si le coté indiqué par le paramètre direction est un mur ou un passage */
	public MazeCellEdge GetEdge (MazeDirection direction) {
		return edges[(int)direction];
	}

	/** Permet de savoir si une cellule a été complétement instanciée */
	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == MazeDirections.Count;
		}
	}

	/** Permet de définir un coté d'une cellule */
	public void SetEdge (MazeDirection direction, MazeCellEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount += 1;
	}

	/** Retourne un coté aléatoire non instancié d'une cellule */
	public MazeDirection RandomUninitializedDirection {
		get {
			int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
			for (int i = 0; i < MazeDirections.Count; i++) {
				if (edges[i] == null) {
					if (skips == 0) {
						return (MazeDirection)i;
					}
					skips -= 1;
				}
			}
			throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
		}
	}
}
