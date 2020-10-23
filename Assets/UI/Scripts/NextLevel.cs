using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
	public int level;
	private int maxLevel = 7;
	GameObject levelSelect; //holds level select UI buttons
	NodeManager nodeManager;
	Text tutorialText;      //references Text object in scene
	GameObject uiMan;
	TableDraw table;

	private void Awake()
	{
		levelSelect = GameObject.Find("LevelSelect");
		nodeManager = GameObject.Find("nodeMan").GetComponent<NodeManager>();   //nodeManager for generating levels
		tutorialText = GameObject.Find("TutorialTip").GetComponent<Text>();
		uiMan = GameObject.Find("uiMan");
		table = GameObject.Find("TableIMG").GetComponent<TableDraw>();
	}

	// Start is called before the first frame update
	void Start()
    {
		GetComponent<Button>().interactable = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void nextLevelPress()
	{
		//setting up next level button
		string levelString = "Level" + (level + 1);
		levelSelect.SetActive(true);
		GameStates nextLevelValues = GameObject.Find(levelString).GetComponent<GameStates>();
		levelSelect.SetActive(false);
		GetComponent<Button>().interactable = false;

		TileManager.Instance.ClearTiles();
		nodeManager.GenerateLevel(nextLevelValues.seed, nextLevelValues.width, nextLevelValues.height, nextLevelValues.secondFluid, nextLevelValues.noRotation);
		tutorialText.text = nextLevelValues.tutorialTextString;
		level = nextLevelValues.level;
		table.DrawTable(nextLevelValues.width, nextLevelValues.height);

		uiMan.GetComponent<ScoreTracking>().LevelEnd();

		uiMan.GetComponent<ScoreTracking>().requiredScore = nextLevelValues.scoreRequirement;

		//if (level == maxLevel)
		//{
		//	GetComponent<Button>().interactable = false;
		//}
	}

	//called on athame button press to unlock next level
	public void UnlockNextLevel()
	{
		//getting next level and unlocking it
		string levelString = "Level" + (level + 1);
		levelSelect.SetActive(true);
		GameObject.Find(levelString).GetComponent<GameStates>().SetChains(false);
		levelSelect.SetActive(false);

		//enabling next level button
		GetComponent<Button>().interactable = true;

	}
}
