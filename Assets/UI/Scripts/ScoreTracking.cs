using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracking : MonoBehaviour
{
    public Text levelScoreLabel;
    public Text totalScoreLabel;
    public Text levelScoreValue;
    public Text totalScoreValue;
    public NodeManager nodeManager;
    public Button endButton;



    public int levelScore = 0; //Current level score
    int totalScore = 0; //Total Score
	public int requiredScore; //score required to pass level
    const int BASE_SCORE = 250;
    // Start is called before the first frame update
    void Start()
    {
		onGUI(true);
    }

    // Update is called once per frame
    void Update()
    {
        //onGUI();
		if (levelScore >= requiredScore)
		{
            
			endButton.interactable = true;
		}
		else
		{
			endButton.interactable = false;
		}
    }

    /// <summary>
    /// Updates all GUI Elements
    /// </summary>
    public void onGUI(bool updateScore)
    {
        totalScoreLabel.text = "Total Score:";
        levelScoreLabel.text = "Level Score:";
		if (updateScore)
		{
			UpdateCurrentScore(nodeManager.paths, false);
		}

        totalScoreValue.text = totalScore.ToString();
        levelScoreValue.text = levelScore.ToString();

        LevelProgressBar.Percentage = Mathf.Min(levelScore / requiredScore, 1);
    }

    /// <summary>
    /// This should be called every frame by the board state, this will update the users score in real time
    /// </summary>
    /// <param name="spellStrength">Used to track what level of spell it is</param>
    /// <param name="bonusConnected">If the bonus tiles have been filled</param>
    public void UpdateCurrentScore(List<Path> paths, bool bonusConnected)
    {
        int spellStrength;
        int pathLength;
        levelScore = 0;
        for (int i = 0; i < paths.Count; i++)
        {

            spellStrength = paths[i].id;
            pathLength = paths[i].Count;
            //If the spell are a level one spell (paths 1 and 2) set the multiplier to 1
            if (spellStrength == 1 || spellStrength == 2)
            {
                spellStrength = 1;
            }
            //If the spell is 3, that means it is a combined spell, set the multiplier to 2
            else if (spellStrength == 3)
            {
                spellStrength = 2;
            }
            else
            {
                Debug.Log("this is where the code breaks");
            }
            levelScore += BASE_SCORE * (spellStrength * pathLength);
            if (bonusConnected)
            {
                levelScore += 200 * spellStrength;
            }
        }
    }

    //Called when the level ends
    public void LevelEnd()
    {
        totalScore += levelScore;
        levelScore = 0;
		onGUI(false);
    }
}
