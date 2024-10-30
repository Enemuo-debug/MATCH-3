using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    public enum ColorType
    {
        PURPLE,
        RED,
        BLUE,   
        ORANGE,
        GREEN,
        YELLOW,
        ANY,
        COUNT
    }
    [System.Serializable]
    public struct ColorOfSprite
    {
        public ColorType color;
        public Sprite sprite;
    }
    private Dictionary<ColorType, Sprite> colorSpriteDictionary; 
    public ColorOfSprite[] colorSprites;
    private ColorType color;
    public ColorType Color
    {
        get {return color;}
        set { SetColor(value);}        
    }
    public int NumColors
    {
        get {return colorSprites.Length;}
    }
    private SpriteRenderer sprite;
    
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        colorSpriteDictionary = new Dictionary<ColorType, Sprite>();
        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDictionary.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDictionary.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDictionary.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDictionary[newColor];
        }
    }
}
