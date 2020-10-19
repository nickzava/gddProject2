using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
	private int level = 1;
	private int maxLevel = 3;
	GameObject levelSelect; //holds level select UI buttons
	NodeManager nodeManager;
	Text tutorialText;      //references Text object in scene
	GameObject uiMan;

	private void Awake()
	{
		levelSelect = GameObject.Find("LevelSelect");
		nodeManager = GameObject.Find("nodeMan").GetComponent<NodeManager>();   //nodeManager for generating levels
		tutorialText = GameObject.Find("TutorialTip").GetComponent<Text>();
		uiMan = GameObject.Find("uiMan");
	}

	// Start is called before the first frame update
	void Start()
    {
        
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
		Debug.Log(levelString);
		GameStates nextLevelValues = GameObject.Find(levelString).GetComponent<GameStates>();
		levelSelect.SetActive(false);

		TileManager.Instance.ClearTiles();
		nodeManager.GenerateLevel(nextLevelValues.seed, nextLevelValues.width, nextLevelValues.height);
		tutorialText.text = nextLevelValues.tutorialTextString;
		level = nextLevelValues.level;
		uiMan.GetComponent<ScoreTracking>().levelScore = 0;
		//uiMan.GetComponent<ScoreTracking>().LevelEnd();
		Debug.Log("back to level select");

		if (level == maxLevel)
		{
			//gameObject.SetActive(false);
			GetComponent<Button>().interactable = false;
		}
	}
}
