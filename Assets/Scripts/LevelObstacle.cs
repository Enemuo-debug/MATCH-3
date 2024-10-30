using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacle : Level
{
    public int numOfMoves;
    private int moveMade = 0;
    public Grid.PieceType obstacleType; 
    private int numOfObstaclesLeft = 0;
    // Start is called before the first frame update
    void Start()
    {
        type = LevelTypes.OBSTACLE;
        for (int i = 0; i < 1; i++)
        {
            numOfObstaclesLeft += grid.GetPiecesOfType(obstacleType).Count;
        }
        hUD.SetLevelType(type);
        hUD.SetRemaining(numOfMoves);
        hUD.SetScore(currentScore);
        hUD.SetTarget(numOfObstaclesLeft);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Moves Remaining: {numOfMoves - moveMade}");
    }
    public override void OnMove()
    {
        base.OnMove();
        moveMade ++;
        hUD.SetRemaining(numOfMoves - moveMade);
        if (numOfMoves - moveMade == 0 && numOfObstaclesLeft > 0)
        {
            GameLose();
        }
    }
    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);
        for (int i = 0; i < 1; i++)
        {
            if (obstacleType == piece.Type)
            {
                numOfObstaclesLeft --; 
                hUD.SetTarget(numOfObstaclesLeft);
                if (numOfObstaclesLeft == 0)
                {
                    currentScore += 1000 * (numOfMoves - moveMade);
                    GameWin();
                    hUD.SetScore(currentScore);
                }
            }
        }
    }
}
