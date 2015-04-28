using UnityEngine;

/** Classe permettant de définir les cotés d'une cellule */
public abstract class MazeCellEdge : MonoBehaviour {
	
	public MazeCell cell, otherCell;
	public MazeDirection direction;

	/** Initialise un coté d'une cellule */
	public void Initialize (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		this.cell = cell;
		this.otherCell = otherCell;
		this.direction = direction;
		cell.SetEdge(direction, this);
		transform.parent = cell.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = direction.ToRotation();
	}
}