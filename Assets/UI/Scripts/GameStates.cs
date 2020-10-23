﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStates : MonoBehaviour
{

	public enum GameState { MainMenu, LevelSelect, InLevel }

	GameObject mainMenu;    //holds main menu UI buttons
	GameObject levelSelect; //holds level select UI buttons
	GameObject inLevel;		//holds in level UI buttons
	NodeManager nodeManager;
	Text tutorialText;      //references Text object in scene
	GameObject uiMan;
	NextLevel nextLevel;
	AudioSource soundEffect;
	TableDraw table;
	Image chain1;
	Image chain2;

	public GameState StateToTransition;
	public int seed;
	public int width;
	public int height;
	public string tutorialTextString;   //string to set tutorial text too
	public int level;
	public int scoreRequirement;
	public bool locked;     //true if level is locked
	public bool secondFluid;
	public bool noRotation; //if true, level will generate non-rotatable tiles

	private void Awake()
	{
		mainMenu = GameObject.Find("MainMenu");										//main menu UI container
		levelSelect = GameObject.Find("LevelSelect");                               //level select UI container
		inLevel = GameObject.Find("InLevel");										//in level UI container
		nodeManager = GameObject.Find("nodeMan").GetComponent<NodeManager>();   //nodeManager for generating levels
		tutorialText = GameObject.Find("TutorialTip").GetComponent<Text>();
		uiMan = GameObject.Find("uiMan");
		table = GameObject.Find("TableIMG").GetComponent<TableDraw>();
		//set chain values
		var temp = transform.Find("LeftChain");
		if (temp != null)
		{
			chain1 = temp.GetComponent<Image>();
		}
		temp = transform.Find("RightChain");
		if (temp != null)
		{
			chain2 = temp.GetComponent<Image>();
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		if (gameObject.name == "MainMenuButton")
		{
			levelSelect.SetActive(false);
			inLevel.SetActive(false);
			table.DisabeTable();
			return;
		}
		SetChains(locked);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	//Transitions to selected state
	public void OnStateTransition()
	{
		switch (StateToTransition)
		{
			case GameState.MainMenu:
				mainMenu.SetActive(true);
				levelSelect.SetActive(false);
				inLevel.SetActive(false);
				table.DisabeTable();
				break;
			case GameState.LevelSelect:
				mainMenu.SetActive(false);
				levelSelect.SetActive(true);
				inLevel.SetActive(false);
				TileManager.Instance.ClearTiles();
				uiMan.GetComponent<ScoreTracking>().LevelEnd();
				table.DisabeTable();
				break;
			case GameState.InLevel:
				mainMenu.SetActive(false);
				levelSelect.SetActive(false);
				inLevel.SetActive(true);
				TileManager.Instance.ClearTiles();
				nodeManager.GenerateLevel(seed, width, height, secondFluid, noRotation);
				table.DrawTable(width, height);
				tutorialText.text = tutorialTextString;
				uiMan.GetComponent<ScoreTracking>().requiredScore = scoreRequirement;
				nextLevel.level = level;
				break;
		}
	}

	public void PlaySound()
	{
		soundEffect.Play();
		Debug.Log("sound effect played");
	}

	public void SetChains(bool visable)
	{
		if (chain1 != null)
		{
			chain1.enabled = visable;
		}
		if (chain2 != null)
		{
			chain2.enabled = visable;
		}

		GetComponent<Button>().interactable = !visable;
	}
}
