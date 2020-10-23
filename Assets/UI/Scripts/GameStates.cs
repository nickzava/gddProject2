using System.Collections;
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

	public GameState StateToTransition;
	public int seed;
	public int width;
	public int height;
	public string tutorialTextString;   //string to set tutorial text too
	public int level;
	public int scoreRequirement;
	public bool locked;		//true if level is locked

	private void Awake()
	{
		mainMenu = GameObject.Find("MainMenu");										//main menu UI container
		levelSelect = GameObject.Find("LevelSelect");                               //level select UI container
		inLevel = GameObject.Find("InLevel");										//in level UI container
		nodeManager = GameObject.Find("nodeMan").GetComponent<NodeManager>();   //nodeManager for generating levels
		tutorialText = GameObject.Find("TutorialTip").GetComponent<Text>();
		uiMan = GameObject.Find("uiMan");
		nextLevel = GameObject.Find("NextLevel").GetComponent<NextLevel>();
		soundEffect = GetComponent<AudioSource>();
	}

	// Start is called before the first frame update
	void Start()
    {
		if (gameObject.name == "MainMenuButton")
		{
			levelSelect.SetActive(false);
			inLevel.SetActive(false);
		}

		if (locked)
		{
			GetComponent<Button>().interactable = false;
		}
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
				break;
			case GameState.LevelSelect:
				mainMenu.SetActive(false);
				levelSelect.SetActive(true);
				inLevel.SetActive(false);
				TileManager.Instance.ClearTiles();
				uiMan.GetComponent<ScoreTracking>().LevelEnd();
				break;
			case GameState.InLevel:
				mainMenu.SetActive(false);
				levelSelect.SetActive(false);
				inLevel.SetActive(true);
				TileManager.Instance.ClearTiles();
				nodeManager.GenerateLevel(seed, width, height);
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
}
