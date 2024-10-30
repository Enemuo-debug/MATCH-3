using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelTypes
    {
            TIMER,
            OBSTACLE,
            MOVES, 
    };
    protected LevelTypes type;
    public LevelTypes Types
    {
        get { return type;}
    }
    public Grid grid;
    public HUD hUD;
    protected int currentScore;
    protected bool didWIn;
    // Start is called before the first frame update
    void Start()
    {
        hUD.SetScore(currentScore); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void GameWin()
    {
        grid.GameOver();
        didWIn = true;
        StartCoroutine(WaitForGridFill());
    }
    public virtual void GameLose()
    {
        grid.GameOver();
        didWIn = false;
        StartCoroutine(WaitForGridFill());
    }
    public virtual void OnMove()
    {
    
    }
    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScore += piece.score;
        hUD.SetScore(currentScore);
    }
    protected IEnumerator WaitForGridFill()
    {
        while(grid.IsFilling)
        {
            yield return 0;
        }
        if (didWIn)
        {
            hUD.OnGameWin(currentScore);
        }
        else 
        {
            hUD.OnGameLose();
        }
    }
}
