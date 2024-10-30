using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    // get the Game pieces
    private GamePiece piece; 
    private IEnumerator moveCoroutine;
    // Start is called before the first frame update

    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);

        piece.transform.localPosition = piece.GridRef.GetCorrectPosition(newX, newY);
    }
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        piece.X = newX;
        piece.Y = newY;
        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.GetCorrectPosition(newX, newY);

        for (float j = 0; j <= 1 * time; j += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, j / time);
            yield return 0;
        }
        piece.transform.position = endPos;

    }
}
