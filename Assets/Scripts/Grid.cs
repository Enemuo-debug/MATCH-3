using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;


public class Grid : MonoBehaviour
{
    public enum PieceType 
    {
        EMPTY,
        NORMAL,
        ROW_CLEAR,
        COLUMN_CLEAR,
        MAGNIFICENT,
        BUBBLE,
        COUNT,
    }
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType pieceType;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    }
    // an array to hold all my piece types and the game object
    public PiecePosition[] initialPieces;
    public PiecePrefab[] piecePrefabs;

    public GameObject bgPrefab;
    public AudioSource swipeAudioPlayer;
    public AudioClip swipeAudio;
    private GamePiece[,] pieces;

    private Dictionary<PieceType, GameObject> prefabDictionary;

    // the x and y dimension of our grid 
    public int xDim;
    public int yDim;
    public GamePiece pressedPiece;
    public GamePiece enteredPiece;
    // reference to our level class 
    public Level level;
    public bool gameOver;
    private bool isFilling = false;
    public bool IsFilling
    {
        get { return isFilling; }
    }
    private float fillTime = 0.1f;
    private bool inverse = false;
    // Start is called before the first frame update
    void Awake()
    {
        pieces = new GamePiece[xDim,yDim];
        prefabDictionary = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!prefabDictionary.ContainsKey(piecePrefabs[i].pieceType))
            {
                prefabDictionary.Add(piecePrefabs[i].pieceType, piecePrefabs[i].prefab);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject bg = (GameObject)Instantiate(bgPrefab, GetCorrectPosition(x, y), bgPrefab.transform.rotation);
                bg.transform.parent = transform;
            }
        }
        for (int i = 0; i < initialPieces.Length; i++)
        {
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }

        for (int x = 0; x < xDim; x ++)
        {
            for (int y = 0; y < yDim; y ++)
            {
                if (pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }
        StartCoroutine(Fill());

        

        
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator Fill()
    {
        bool needRefill = true;
        isFilling = true;
        while(needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while(FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needRefill = ClearAllValidMatches();
        }
        isFilling = false;
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagPiece = pieces[diagX, y +1];
                                    if (diagPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }

                                        }
                                        if (!hasPieceAbove)
                                        {
                                            Destroy (diagPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y+1, fillTime);
                                            pieces[diagX, y+1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            
                            }
                        }
                    }
                }
            }
        }
        for (int x = 0; x < xDim; x++)
        {
           GamePiece pieceBelow = pieces[x, 0];
           if (pieceBelow.Type == PieceType.EMPTY)
           {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(prefabDictionary[PieceType.NORMAL], GetCorrectPosition(x, -1), prefabDictionary[PieceType.NORMAL].transform.rotation);
                newPiece.transform.parent = transform;
                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this , PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)UnityEngine.Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
           } 
        }
        return movedPiece;
    }
    public Vector2 GetCorrectPosition(int x, int y)
    {
        return new Vector2(transform.position.x -xDim/2 + x, transform.position.y - yDim/2 + y);
    }
    public GamePiece SpawnNewPiece(int x, int y, PieceType pieceType)
    {
        GameObject newPiece = (GameObject)Instantiate(prefabDictionary[pieceType], GetCorrectPosition(x, y), prefabDictionary[pieceType].transform.rotation);
        newPiece.transform.parent = transform;
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this , pieceType);
        return pieces[x, y];
    }
    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return ((piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1));
    }
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver == true)
            return;
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;  
            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null||
             piece1.Type == PieceType.MAGNIFICENT || piece2.Type == PieceType.MAGNIFICENT)
            {                
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);
                if (piece1.Type == PieceType.MAGNIFICENT && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();
                    if (clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }
                    ClearPiece(piece1.X, piece1.Y);
                }
                 if (piece2.Type == PieceType.MAGNIFICENT && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();
                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }
                    ClearPiece(piece2.X, piece2.Y);
                }
                ClearAllValidMatches();

                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }

                pressedPiece = null;
                enteredPiece = null;
                StartCoroutine(Fill());
                level.OnMove(); 
                swipeAudioPlayer.PlayOneShot(swipeAudio);
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }  

        }
    }
    public void PressedPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }
    public void EnteredPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }
    public void ReleasedPiece()
    {
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }
    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //check for match in horizontal
            horizontalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir ++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset ++)
                {
                    int x;
                    if (dir == 0) // going left
                    {
                        x = newX - xOffset;
                    }
                    else // going right 
                    {
                        x = newX + xOffset;
                    }
                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }
                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else 
                    {
                        break;
                    }
                }
            }
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }
            // Traverse vertically after we have found a match in our horizontal line (to check for L and T space)
            if (horizontalPieces.Count >= 3)
            {   for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir ++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++ )
                        {
                            int y;
                            if (dir == 0) // going up
                            { 
                                y = newY - yOffset;
                            }
                            else // going down
                            {
                                y = newY + yOffset;
                            }
                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }
                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            //check for match in vertical
            verticalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir ++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset ++)
                {
                    int y;
                    if (dir == 0) // going up
                    { 
                        y = newY - yOffset;
                    }
                    else // going down
                    {
                        y = newY + yOffset;
                    }
                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else 
                    {
                        break;
                    }
                }
            }
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }
            // Traverse horizontally after we have found a match in our vertical line (to check for L and T space)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) // going left
                            {
                                x = newX - xOffset;
                            }
                            else // going right 
                            {
                                x = newX + xOffset;
                            }
                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }
                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }

                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
            
        }   
        return null;
    }
    public bool ClearPiece(int x, int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsCleared)
        {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            ClearObstacles(x, y);

            return true;
        }
        return false;
    } 
    public bool ClearAllValidMatches()
    {
        bool needRefill = false;
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if (match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y; 

                        if (match.Count == 4)
                        {
                            if (pressedPiece == null|| enteredPiece == null)
                            {
                                specialPieceType = (PieceType)UnityEngine.Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);

                            }
                            else if (pressedPiece.Y == enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else 
                            {
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                        }
                        else if (match.Count >= 5)
                        {
                            specialPieceType = PieceType.MAGNIFICENT;
                        }
                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needRefill = true;

                                if (match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;     
                                }
                            }
                        }
                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY].gameObject);
                            GamePiece newpPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR) && newpPiece.IsColored() && match[0].IsColored())
                            {
                                newpPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }
                            else if (specialPieceType == PieceType.MAGNIFICENT && newpPiece.IsColored())
                            {
                                newpPiece.ColorComponent.SetColor(ColorPiece.ColorType.ANY);
                            } 
                        }
                    }
                }
            }
        }
        return needRefill;
    }
    public void ClearObstacles(int x, int y)
    {
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BUBBLE && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }
        for (int adjacentY = y - 1;  adjacentY <= y + 1; adjacentY++)
        {
           if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
           {
                if (pieces[x, adjacentY].Type == PieceType.BUBBLE && pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
           }
        }
    }
    public void ClearRow(int row)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }
    public void ClearColumn(int col)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(col, y);
        }
    }
    public void ClearColor(ColorPiece.ColorType color)
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].IsColored() && pieces[x, y].ColorComponent.Color == color || color == ColorPiece.ColorType.ANY)
                {
                    ClearPiece(x, y);
                }
            }
        }
    } 
    public void GameOver()
    {
        gameOver = true;
    }
    public List<GamePiece> GetPiecesOfType(PieceType type)
    {
        List<GamePiece> piecesOfType = new List<GamePiece>();
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].Type == type)
                {
                    piecesOfType.Add(pieces[x, y]);
                }
            }
        }
        return piecesOfType;
    }
}
