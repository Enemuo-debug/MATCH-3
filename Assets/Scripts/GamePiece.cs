using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    // state thy variables 
    public int score;
    private int x;
    private int y;
    public int X
    {
        get{return x;}
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get {return y;}
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }
    private Grid.PieceType type;
    public Grid.PieceType Type
    {
        get {return type;}
    }
    private Grid grid;
    public Grid GridRef
    {
        get {return grid;}
    }
    private Movable movableComponent;
    public Movable MovableComponent
    {
        get {return movableComponent;}
    }
    private ColorPiece colorComponent;
    public ColorPiece ColorComponent
    {
        get {return colorComponent;}
    }
    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent
    {
        get {return clearableComponent;}
    }

    void Awake()
    {
        movableComponent = GetComponent<Movable>();
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init(int _x, int _y, Grid _grid, Grid.PieceType _pieceType)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type =_pieceType;
    }
    public bool IsMovable()
    {
        return movableComponent != null;
    }
    public bool IsColored()
    {
        return colorComponent != null;
    }
    public bool IsClearable()
    {
        return clearableComponent != null;
    }
    void OnMouseDown()
    {
        grid.PressedPiece(this);
    }
    void OnMouseUp()
    {
        grid.ReleasedPiece();
    }
    void OnMouseEnter()
    {
        grid.EnteredPiece(this);
    }
}
