using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class LevelMove : Level
{
    public int numOfMoves;
    public int targetScore;
    private int moveMade = 0;
    // Start is called before the first frame update
    void Start()
    {
        type = LevelTypes.MOVES;
        hUD.SetLevelType(type);
        hUD.SetScore(currentScore);
        hUD.SetTarget(targetScore);
        hUD.SetRemaining(numOfMoves);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnMove()
    {
        base.OnMove();
        moveMade ++;
        hUD.SetRemaining(numOfMoves - moveMade);
        if (numOfMoves - moveMade == 0)
        {
            if (currentScore >= targetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }
    }
}
