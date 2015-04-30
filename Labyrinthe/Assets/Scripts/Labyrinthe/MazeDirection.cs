using UnityEngine;

/** Enum définissant les diférentes directions possible d'une cellules */
public enum MazeDirection {
	North,
	East,
	South,
	West
}

/** Classe static définissant les directions d'une cellules */
public static class MazeDirections {
	
	public const int Count = 4;

	/** Retourne une direction aléatoire */
	public static MazeDirection RandomValue {
		get {
			return (MazeDirection)Random.Range(0, Count);
		}
	}

	/** Contient les différentes directions sous forme de vecteurs */
	private static IntVector2[] vectors = {
		new IntVector2(0, 1),
		new IntVector2(1, 0),
		new IntVector2(0, -1),
		new IntVector2(-1, 0)
	};

	/** Contient les différentes directions sous forme d'Enum */
	private static MazeDirection[] opposites = {
		MazeDirection.South,
		MazeDirection.West,
		MazeDirection.North,
		MazeDirection.East
	};

	/** Retourne la direction opposé à celle passée en paramètre */
	public static MazeDirection GetOpposite (this MazeDirection direction) {
		return opposites[(int)direction];
	}

	/** Retourne un IntVector2 en fonction d'une direction passée en paramètre */
	public static IntVector2 ToIntVector2 (this MazeDirection direction) {
		return vectors[(int)direction];
	}

	/** Contient les différentes rotations sous forme de Quaternion */
	private static Quaternion[] rotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};

	/** Retourne un Quaternion correspondant à la direction passée en paramètre */
	public static Quaternion ToRotation (this MazeDirection direction) {
		return rotations[(int)direction];
	}
}