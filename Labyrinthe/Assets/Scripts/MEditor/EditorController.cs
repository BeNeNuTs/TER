using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.IO;

public class EditorController : MonoBehaviour {

	public Slider widthSlider;
	public Slider heightSlider;

	public Button saveButton;
	public Button solveButton;

	public GameObject mazeGameObject;

	private Maze maze;
	private PlateauController plateauScript;
	private DeadEndFilling deadEndScript;

	private string lines;
	private string columns;

	void Start(){
		maze = mazeGameObject.GetComponent<Maze>();
		plateauScript = mazeGameObject.GetComponent<PlateauController>();
		deadEndScript = GetComponent<DeadEndFilling>();
	}

	public void Generate(){
		if(!saveButton.IsInteractable()){
			saveButton.interactable = true;
		}
		if(!solveButton.IsInteractable()){
			solveButton.interactable = true;
		}

		//Supprimer l'ancien labyrinthe généré
		Transform [] child = maze.transform.GetComponentsInChildren<Transform>();
		foreach(Transform c in child){
			if(c != maze.transform)
				Destroy(c.gameObject);
		}
		//////////////////////////////////////

		ResetRotationLabyrinthe();

		//Récupérer le width & height des sliders et générer le labyrinthe
		int width = Mathf.FloorToInt(widthSlider.value);
		int height = Mathf.FloorToInt(heightSlider.value);
		maze.GenerateNoCoroutine(new IntVector2(width, height));

		SetGlobalView(width, height);		
	}

	public static void SetGlobalView(int width, int height){
		//Déplacer la caméra au bon endroit afin de voir le labyrinthe généré
		int max = Mathf.Max(width, height);
		Vector3 newPos = new Vector3(Camera.main.transform.position.x,max + max/4f,Camera.main.transform.position.z);
		if(Camera.main.transform.position != newPos)
			iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", newPos, "time", 2f));
	}

	public void Solve(){
		solveButton.interactable = false;

		ResetRotationLabyrinthe();
		deadEndScript.deadEndFilling(maze, new IntVector2(0,0), new IntVector2(maze.size.x - 1, maze.size.z - 1));
	}

	public void Save(){
		if(iTween.Count() > 0){
			return;
		}

		saveButton.interactable = false;
		deadEndScript.clear();
		ResetRotationLabyrinthe();

		XmlTextReader myXmlTextReader = LabyrintheManager.GetSavedLevelXML();

		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(myXmlTextReader);

		myXmlTextReader.Close();

		XmlNode savedLevel = xdoc.SelectSingleNode("//savedLevels");
		XmlNode level = xdoc.LastChild;

		//Level node
		XmlNode xmlNewLevel = xdoc.CreateNode(XmlNodeType.Element, "level", null);
		XmlAttribute id = xdoc.CreateAttribute("id");
		id.Value = savedLevel.ChildNodes.Count.ToString();
		XmlAttribute img = xdoc.CreateAttribute("img");
		img.Value = "img/" + id.Value;
		XmlAttribute name = xdoc.CreateAttribute("name");
		name.Value = "test";
		xmlNewLevel.Attributes.Append(id);
		xmlNewLevel.Attributes.Append(img);
		xmlNewLevel.Attributes.Append(name);

		//SCREENSHOT MAZE
		int screenWidth = Mathf.FloorToInt(Camera.main.rect.width * Screen.width);
		int screenHeight = Mathf.FloorToInt(Camera.main.rect.height * Screen.height);

		RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24);
		Camera.main.targetTexture = rt;
		Texture2D screenShot = new Texture2D(Mathf.FloorToInt(screenWidth * Camera.main.rect.width), screenHeight, TextureFormat.RGB24, false);
		Camera.main.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, Mathf.FloorToInt(screenWidth * Camera.main.rect.width), screenHeight), 0, 0);
		
		Camera.main.targetTexture = null;
		RenderTexture.active = null; // JC: added to avoid errors
		Destroy(rt);
		
		byte[] bytes = screenShot.EncodeToPNG();
		string filename = Application.dataPath + "/Resources/img/" + id.Value + ".png";
		System.IO.File.WriteAllBytes(filename, bytes);
		//////////////

		//PosBille node
		XmlNode xmlNewPosBille = xdoc.CreateNode(XmlNodeType.Element, "posBille", null);
		XmlAttribute x = xdoc.CreateAttribute("x");
		x.Value = "0";
		XmlAttribute y = xdoc.CreateAttribute("y");
		y.Value = "0";
		xmlNewPosBille.Attributes.Append(x);
		xmlNewPosBille.Attributes.Append(y);
		//////////////
		 
		//PosExit node
		XmlNode xmlNewPosExit = xdoc.CreateNode(XmlNodeType.Element, "posExit", null);
		x = xdoc.CreateAttribute("x");
		x.Value = (Mathf.FloorToInt(widthSlider.value) - 1).ToString();
		y = xdoc.CreateAttribute("y");
		y.Value = (Mathf.FloorToInt(heightSlider.value) - 1).ToString();
		xmlNewPosExit.Attributes.Append(x);
		xmlNewPosExit.Attributes.Append(y);
		//////////////

		//Labyrinthe node
		XmlNode xmlNewLabyrinthe = xdoc.CreateNode(XmlNodeType.Element, "labyrinthe", null);
		XmlAttribute width = xdoc.CreateAttribute("width");
		width.Value = Mathf.FloorToInt(widthSlider.value).ToString();
		XmlAttribute height = xdoc.CreateAttribute("height");
		height.Value = Mathf.FloorToInt(heightSlider.value).ToString();
		xmlNewLabyrinthe.Attributes.Append(width);
		xmlNewLabyrinthe.Attributes.Append(height);

		XmlNode xmlNewLines = xdoc.CreateNode(XmlNodeType.Element, "lines", null);
		XmlNode xmlNewColumns = xdoc.CreateNode(XmlNodeType.Element, "columns", null);

		FormatMaze();
		xmlNewLines.InnerText = lines;
		xmlNewColumns.InnerText = columns;
		xmlNewLabyrinthe.AppendChild(xmlNewLines);
		xmlNewLabyrinthe.AppendChild(xmlNewColumns);
		//////////////

		//Time node
		XmlNode xmlNewTime = xdoc.CreateNode(XmlNodeType.Element, "time", null);
		XmlAttribute timeGold = xdoc.CreateAttribute("gold");
		timeGold.Value = "10";
		XmlAttribute timeSilver = xdoc.CreateAttribute("silver");
		timeSilver.Value = "15";
		XmlAttribute timeBronze = xdoc.CreateAttribute("bronze");
		timeBronze.Value = "20";
		xmlNewTime.Attributes.Append(timeGold);
		xmlNewTime.Attributes.Append(timeSilver);
		xmlNewTime.Attributes.Append(timeBronze);
		//////////////

		xmlNewLevel.AppendChild(xmlNewPosBille);
		xmlNewLevel.AppendChild(xmlNewPosExit);
		xmlNewLevel.AppendChild(xmlNewLabyrinthe);
		xmlNewLevel.AppendChild(xmlNewTime);

		savedLevel.InsertAfter(xmlNewLevel, level);

		xdoc.Save(Application.dataPath + LabyrintheManager.folderDocs + LabyrintheManager.folderSave + "/savedLevels.xml");
	}

	private void FormatMaze() {
		lines = "";
		columns = "";

		for(int z = 0 ; z < maze.size.z ; z++){
			for(int x = 0 ; x < maze.size.x - 1 ; x++){
				MazeCell c = maze.GetCell(new IntVector2(x,z));
				if(c.GetEdge(MazeDirection.East).GetType() == typeof(MazeWall)){
					lines += "1";
				}else{
					lines += "0";
				}

				if(x < maze.size.x - 2)
					lines += "-";
			}

			if(z < maze.size.z - 1)
				lines += "|";
		}

		for(int x = 0 ; x < maze.size.x ; x++){
			for(int z = 0 ; z < maze.size.z - 1 ; z++){
				MazeCell c = maze.GetCell(new IntVector2(x,z));
				if(c.GetEdge(MazeDirection.North).GetType() == typeof(MazeWall)){
					columns += "1";
				}else{
					columns += "0";
				}
				
				if(z < maze.size.z - 2)
					columns += "-";
			}
			
			if(x < maze.size.x - 1)
				columns += "|";
		}
	}

	public void BackToMenu(){
		GameController.BackToMenu();
	}

	private void ResetRotationLabyrinthe(){
		//Remettre le labyrinthe en rotation (0,0,0)
		plateauScript.ResetRotation();
		maze.transform.rotation = Quaternion.Euler(0,0,0);
	}

}
