using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    public AnimationClip removeAnim;
    private bool isCleared;
    public bool IsCleared
    {
        get {return isCleared;}
    }
    protected GamePiece piece;
    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Clear()
    {
        isCleared = true;
        piece.GridRef.level.OnPieceCleared(piece);
        StartCoroutine(ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(removeAnim.name);
        }
        yield return new WaitForSeconds(removeAnim.length);
        Destroy(gameObject);
    }
}
