using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{

	public enum GameState { MainMenu, LevelSelect, InLevel }

	GameObject mainMenu;    //holds main menu UI buttons
	GameObject levelSelect; //holds level select UI buttons
	GameObject inLevel;		//holds in level UI buttons
	NodeManager nodeManager;

	public GameState StateToTransition;
	public int seed;
	public int width;
	public int height;

	private void Awake()
	{
		mainMenu = GameObject.Find("MainMenu");
		levelSelect = GameObject.Find("LevelSelect");
		nodeManager = GameObject.Find("NodeManager").GetComponent<NodeManager>();
		inLevel = GameObject.Find("InLevel");
	}

	// Start is called before the first frame update
	void Start()
    {
		if (gameObject.name == "MainMenuButton")
		{
			levelSelect.SetActive(false);
			inLevel.SetActive(false);
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
				break;
			case GameState.InLevel:
				mainMenu.SetActive(false);
				levelSelect.SetActive(false);
				inLevel.SetActive(true);
				nodeManager.GenerateLevel(seed, width, height);
				break;
		}
	}
}
