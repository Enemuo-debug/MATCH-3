using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEditor;


public class HUD : MonoBehaviour
{
    public Level level;
    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI targetSubText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreSubText;
    public GameOver gameOver;
    public bool  isGameOver;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }
    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }
    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }
    public void SetLevelType(Level.LevelTypes levelTypes)
    {
        if (levelTypes == Level.LevelTypes.MOVES)
        {
            remainingSubText.text = $"Moves Remaining";
            targetSubText.text = $"Target Score";
        }
        else if (levelTypes == Level.LevelTypes.OBSTACLE)
        {
            remainingSubText.text = $"Moves Remaining";
            targetSubText.text = $"Bubbles Remaining";
        }
        else if (levelTypes == Level.LevelTypes.TIMER)
        {
            remainingSubText.text = $"Time Remaining";
            targetSubText.text = $"Target Score";
        }
    }
    public void OnGameWin(int score)
    {
        gameOver.ShowWin(score);
    }
    public void OnGameLose()
    {
        gameOver.ShowLose();
    }
}
