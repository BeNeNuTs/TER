using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	public MazeCell cellPrefab;
	public float generationStepDelay;
	public IntVector2 size;
	public MazePassage passagePrefab;
	public MazeWall wallPrefab;

	private MazeCell[,] cells;

	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}
	
	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazeWall wall = Instantiate(wallPrefab) as MazeWall;
		wall.Initialize(cell, otherCell, direction);
		if (otherCell != null) {
			wall = Instantiate(wallPrefab) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		activeCells.Add(CreateCell(RandomCoordinates));
	}
	
	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];
		if (currentCell.IsFullyInitialized) {
			activeCells.RemoveAt(currentIndex);
			return;
		}
		MazeDirection direction = currentCell.RandomUninitializedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
		if (ContainsCoordinates(coordinates)) {
			MazeCell neighbor = GetCell(coordinates);
			if (neighbor == null) {
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			}
			else {
				CreateWall(currentCell, neighbor, direction);
			}
		}
		else {
			CreateWall(currentCell, null, direction);
		}
	}

	public IEnumerator GenerateLevel(Level level){
		IntVector2 _size = new IntVector2(level.width, level.height);
		size = _size;
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		for(int z = 0 ; z < size.z ; z++){
			for(int x = 0 ; x < size.x ; x++){
				MazeCell c = CreateCell(new IntVector2(x,z));
				activeCells.Add(c);
				if(z == 0){
					CreateWall(c, null, MazeDirection.South);
				}else if(z == size.z - 1){
					CreateWall(c, null, MazeDirection.North);
				}

				if(x == 0){
					CreateWall(c, null, MazeDirection.West);
				}else if(x == size.x - 1){
					CreateWall(c, null, MazeDirection.East);
				}
			}
		}

		string [] line;
		string [] column;
		for(int x = 0 ; x < level.lines.Length ; x++){
			line = level.lines[x].Split('-');
			if(line.Length + 1 != size.x){
				Debug.LogError("Nombre de mur insuffisant par rapport à la taille du labyrinthe : line");
				yield return null;
			}
			
			for(int i = 0 ; i < line.Length ; i++){
				if(line[i] == "1"){
					CreateWall(activeCells[x * size.x + i], activeCells[x * size.x + i+1], MazeDirection.East);
				}
			}
		}
		for(int z = 0 ; z < level.columns.Length ; z++){
			column = level.columns[z].Split('-');
			if(column.Length + 1 != size.z){
				Debug.LogError("Nombre de mur insuffisant par rapport à la taille du labyrinthe : colum");
				yield return null;
			}

			for(int i = 0 ; i < column.Length ; i++){
				if(column[i] == "1"){
					CreateWall(activeCells[i * size.x + z], activeCells[(i+1) * size.x + z], MazeDirection.North);
				}
			}
		}
	}

	public void GenerateNoCoroutine (IntVector2 _size) {
		size = _size;
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		while (activeCells.Count > 0) {
			DoNextGenerationStep(activeCells);
		}
	}

	public IEnumerator Generate (IntVector2 _size) {
		size = _size;
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		while (activeCells.Count > 0) {
			yield return delay;
			DoNextGenerationStep(activeCells);
		}
	}
	
	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		return newCell;
	}

	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}
	
	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}
}
