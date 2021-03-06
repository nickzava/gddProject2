﻿
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
    Text flavorText;
	GameObject uiMan;
	TableDraw table;

	private void Awake()
	{
		levelSelect = GameObject.Find("LevelSelect");
		nodeManager = GameObject.Find("nodeMan").GetComponent<NodeManager>();   //nodeManager for generating levels
		tutorialText = GameObject.Find("TutorialTip").GetComponent<Text>();
        flavorText = GameObject.Find("LevelScore").GetComponent<Text>();
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
		string levelString;
		if (level != maxLevel)
		{
			levelString = "Level" + (level + 1);
		}
		else
		{
			levelString = "Level" + maxLevel;
		}
		levelSelect.SetActive(true);
		GameStates nextLevelValues = GameObject.Find(levelString).GetComponent<GameStates>();
		levelSelect.SetActive(false);
		GetComponent<Button>().interactable = false;

		TileManager.Instance.ClearTiles();
		if (level != maxLevel)
		{
			nodeManager.GenerateLevel(nextLevelValues.seed, nextLevelValues.width, nextLevelValues.height, nextLevelValues.secondFluid, nextLevelValues.noRotation);
		}
		else
		{
			nodeManager.GenerateLevel((int)Random.Range(0, int.MaxValue - 1) , nextLevelValues.width, nextLevelValues.height, nextLevelValues.secondFluid, nextLevelValues.noRotation);
		}
		tutorialText.text = nextLevelValues.tutorialTextString;
        flavorText.text = nextLevelValues.flavorTextString;
		level = nextLevelValues.level;
		table.DrawTable(nextLevelValues.width, nextLevelValues.height);

		uiMan.GetComponent<ScoreTracking>().LevelEnd();

		uiMan.GetComponent<ScoreTracking>().requiredScore = nextLevelValues.scoreRequirement;

	}

	//called on athame button press to unlock next level
	public void UnlockNextLevel()
	{
		if (level != maxLevel)
		{
			//getting next level and unlocking it
			string levelString = "Level" + (level + 1);
			levelSelect.SetActive(true);
			GameObject.Find(levelString).GetComponent<GameStates>().SetChains(false);
			levelSelect.SetActive(false);
		}

		//enabling next level button
		GetComponent<Button>().interactable = true;
	}
}
