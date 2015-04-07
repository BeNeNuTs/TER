using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.IO;

public class EditorController : MonoBehaviour {

	public InputField nameLab;

	public Slider widthSlider;
	public Slider heightSlider;

	public InputField posBilleX;
	public InputField posBilleZ;

	public InputField posExitX;
	public InputField posExitZ;

	public InputField timeGold;
	public InputField timeSilver;
	public InputField timeBronze;

	public Button saveButton;
	public Button solveButton;

	public Text errorText;

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
		CheckAllField();
		CheckPosField();

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

		int posXBille = int.Parse(posBilleX.text);
		int posZBille = int.Parse(posBilleZ.text);

		int posXExit = int.Parse(posExitX.text);
		int posZExit = int.Parse(posExitZ.text);
		deadEndScript.deadEndFilling(maze, new IntVector2(posXBille, posZBille), new IntVector2(posXExit, posZExit));
	}

	public void Save(){
		if(iTween.Count() > 0){
			return;
		}

		FormatMaze();
		if(LevelAlreadyExist(new IntVector2(int.Parse(posBilleX.text), int.Parse(posBilleZ.text)), new IntVector2(int.Parse(posExitX.text), int.Parse(posExitZ.text)), Mathf.FloorToInt(widthSlider.value), Mathf.FloorToInt(heightSlider.value), lines, columns)){
			ShowError("Le niveau existe déjà.");
			//Debug.LogError("Level already exist");
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
		name.Value = nameLab.text;
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
		x.Value = posBilleX.text;
		XmlAttribute y = xdoc.CreateAttribute("y");
		y.Value = posBilleZ.text;
		xmlNewPosBille.Attributes.Append(x);
		xmlNewPosBille.Attributes.Append(y);
		//////////////
		 
		//PosExit node
		XmlNode xmlNewPosExit = xdoc.CreateNode(XmlNodeType.Element, "posExit", null);
		x = xdoc.CreateAttribute("x");
		x.Value = posExitX.text;
		y = xdoc.CreateAttribute("y");
		y.Value = posExitZ.text;
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

		xmlNewLines.InnerText = lines;
		xmlNewColumns.InnerText = columns;
		xmlNewLabyrinthe.AppendChild(xmlNewLines);
		xmlNewLabyrinthe.AppendChild(xmlNewColumns);
		//////////////

		//Time node
		XmlNode xmlNewTime = xdoc.CreateNode(XmlNodeType.Element, "time", null);
		XmlAttribute timeGoldXml = xdoc.CreateAttribute("gold");
		timeGoldXml.Value = timeGold.text;
		XmlAttribute timeSilverXml = xdoc.CreateAttribute("silver");
		timeSilverXml.Value = timeSilver.text;
		XmlAttribute timeBronzeXml = xdoc.CreateAttribute("bronze");
		timeBronzeXml.Value = timeBronze.text;
		xmlNewTime.Attributes.Append(timeGoldXml);
		xmlNewTime.Attributes.Append(timeSilverXml);
		xmlNewTime.Attributes.Append(timeBronzeXml);
		//////////////

		xmlNewLevel.AppendChild(xmlNewPosBille);
		xmlNewLevel.AppendChild(xmlNewPosExit);
		xmlNewLevel.AppendChild(xmlNewLabyrinthe);
		xmlNewLevel.AppendChild(xmlNewTime);

		savedLevel.InsertAfter(xmlNewLevel, level);

		xdoc.Save(Application.dataPath + LabyrintheManager.folderDocs + LabyrintheManager.folderSave + "/savedLevels.xml");
	}

	public static void RemoveLevel(int idLevel){
		XmlTextReader myXmlTextReader = LabyrintheManager.GetSavedLevelXML();
		
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(myXmlTextReader);
		
		myXmlTextReader.Close();

		XmlNodeList levelNodes = xdoc.GetElementsByTagName("level");
		
		for (int i = 0; i < levelNodes.Count; i++)
		{
			if (levelNodes[i].Attributes["id"] != null)
			{
				if (idLevel.ToString() == levelNodes[i].Attributes["id"].InnerText)
				{    
					levelNodes[i].ParentNode.RemoveChild(levelNodes[i]);
					xdoc.Save(Application.dataPath + LabyrintheManager.folderDocs + LabyrintheManager.folderSave + "/savedLevels.xml");
					Debug.Log("Level id = " + idLevel + " was removed.");
					return;
				}
			}
		}

		Debug.LogError("Level to remove doesn't find.");
	}

	public static bool LevelAlreadyExist(IntVector2 posBille, IntVector2 posExit, int width, int height, string lines, string columns){
		XmlTextReader myXmlTextReader = LabyrintheManager.GetSavedLevelXML();
		
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(myXmlTextReader);
		
		myXmlTextReader.Close();
		
		XmlNodeList levelNodes = xdoc.GetElementsByTagName("level");
		
		for (int i = 0; i < levelNodes.Count; i++)
		{
			XmlNode posBilleNode = levelNodes[i].FirstChild;
			XmlNode posExitNode = levelNodes[i]["posExit"];
			XmlNode labyrintheNode = levelNodes[i]["labyrinthe"];
			XmlNode linesNode = labyrintheNode.FirstChild;
			XmlNode columnsNode = labyrintheNode.LastChild;

			if (posBilleNode.Attributes["x"].InnerText == posBille.x.ToString() && posBilleNode.Attributes["y"].InnerText == posBille.z.ToString())
			{  
				if (posExitNode.Attributes["x"].InnerText == posExit.x.ToString() && posExitNode.Attributes["y"].InnerText == posExit.z.ToString())
				{ 
					if (labyrintheNode.Attributes["width"].InnerText == width.ToString() && labyrintheNode.Attributes["height"].InnerText == height.ToString())
					{
						if (linesNode.InnerText == lines && columnsNode.InnerText == columns)
						{
							return true;
						}
					}
				}
			}
		}

		return false;
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

	public void CheckPosField(){
		deadEndScript.clear();

		if(CheckPosBille() && CheckPosExit()){
			solveButton.interactable = true;
		}else{
			solveButton.interactable = false;
		}
	}

	public void CheckAllField(){
		if(CheckName() && CheckPosBille() && CheckPosExit() && CheckTime()){
			saveButton.interactable = true;
		}else{
			saveButton.interactable = false;
		}
	}

	public bool CheckName(){
		if(nameLab.text == ""){
			ShowError("Nom incorrect.");
			return false;
		}else{
			return true;
		}
	}

	public bool CheckPosBille(){
		if(posBilleX.text == "" || posBilleZ.text == ""){
			return false;
		}

		int posX = int.Parse(posBilleX.text);
		int posZ = int.Parse(posBilleZ.text);

		int width = Mathf.FloorToInt(widthSlider.value);
		int height = Mathf.FloorToInt(heightSlider.value);

		if(posX > width - 1 || posZ > height - 1){
			ShowError("Position bille incorrecte.");
			return false;
		}else{
			return true;
		}
	}

	public bool CheckPosExit(){
		if(posExitX.text == "" || posExitZ.text == ""){
			return false;
		}

		int posX = int.Parse(posExitX.text);
		int posZ = int.Parse(posExitZ.text);
		
		int width = Mathf.FloorToInt(widthSlider.value);
		int height = Mathf.FloorToInt(heightSlider.value);
		
		if(posX > width - 1 || posZ > height - 1){
			ShowError("Position sortie incorrecte.");
			return false;
		}else{
			return true;
		}
	}

	public bool CheckTime(){
		if(timeGold.text == "" || timeSilver.text == "" || timeBronze.text == ""){
			return false;
		}

		int gold = int.Parse(timeGold.text);
		int silver = int.Parse(timeSilver.text);
		int bronze = int.Parse(timeBronze.text);

		if(gold < silver && silver < bronze){
			return true;
		}else{
			ShowError("Temps incorrect.");
			return false;
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

	public void ShowError(string error){

		Animator anim = errorText.gameObject.GetComponent<Animator>();

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fade"))
		{
			if(!errorText.text.Contains(error))
				errorText.text += "\n"+error;
		}else{
			errorText.text = error;
		}

		anim.SetTrigger("fade");
	}

}
