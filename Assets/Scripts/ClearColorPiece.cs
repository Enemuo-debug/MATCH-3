using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColorPiece : ClearablePiece
{
    private ColorPiece.ColorType color;
    public ColorPiece.ColorType Color
    {
        get { return color;}
        set { color = value;}
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Clear()
    {
        base.Clear();
        // clear pieces of specific color 
        piece.GridRef.ClearColor(color);
    }
}
