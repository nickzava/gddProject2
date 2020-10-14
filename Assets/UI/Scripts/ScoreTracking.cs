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

    int levelScore = 0; //Current level score
    int totalScore = 0; //Total Score
    const int BASE_SCORE = 250;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onGUI();
    }

    /// <summary>
    /// Updates all GUI Elements
    /// </summary>
    void onGUI()
    {
        
        totalScoreLabel.text = "Total Score:";
        levelScoreLabel.text = "Level Score:";

        UpdateCurrentScore(nodeManager.paths, false);
        totalScoreValue.text = totalScore.ToString();
        levelScoreValue.text = levelScore.ToString();

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
            pathLength = paths[i].GetLongestSequenceInPath().Count;
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
    }
}
