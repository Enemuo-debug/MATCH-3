using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;


public class LevelTimer : Level
{
    public int timeInSecs;
    private float timer;
    public int targetScore;
    public bool timeOut;
    // Start is called before the first frame update
    void Start()
    {
        timeOut = false;
        type = LevelTypes.TIMER;
        hUD.SetLevelType(type);
        hUD.SetRemaining(string.Format("{0}:{1:00}", timeInSecs/60, timeInSecs % 60));
        hUD.SetScore(currentScore);
        hUD.SetTarget(targetScore);

    }

    // Update is called once per frame
    void Update()
    {
        if (!timeOut)
        {
            timer += Time.deltaTime;
            hUD.SetRemaining(string.Format("{0}:{1:00}", (int)Mathf.Max((timeInSecs - timer)/60, 0), (int)Mathf.Max((timeInSecs - timer) % 60), 0));
            if (timeInSecs - timer <= 0)
            {
                if (currentScore >= targetScore)
                {
                    GameWin();
                    timeOut = true;
                }
                else
                {
                    GameLose();
                    timeOut = true;
                }
            }

        }
        
    }
}
