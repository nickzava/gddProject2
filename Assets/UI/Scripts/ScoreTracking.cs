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
        totalScore = 100000;
        levelScore = 2000000;
        totalScoreValue.text = totalScore.ToString();
        levelScoreValue.text = levelScore.ToString();

    }

    /// <summary>
    /// This should be called every frame by the board state, this will update the users score in real time
    /// </summary>
    /// <param name="spellStrength">Used to track what level of spell it is</param>
    /// <param name="bonusConnected">If the bonus tiles have been filled</param>
    public void UpdateCurrentScore(int spellStrength, bool bonusConnected)
    {
        levelScore += BASE_SCORE * (spellStrength * 3);
        if (bonusConnected)
        {
            levelScore += 200 * spellStrength;
        }
    }

    //Called when the level ends
    public void LevelEnd()
    {
        totalScore += levelScore;
        levelScore = 0;
    }
}
