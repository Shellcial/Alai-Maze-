using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapSetColor : MonoBehaviour
{
    private Tilemap tilemap;
    private Color tilemapBoarder = new Color (0.5482014f, 0.5438849f, 0.6f, 1);
    private Color tilemapWallColor = new Color(0.85f, 0.85f, 0.85f, 1);
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name == "Outside_Boarder_Tilemap")
        {
            tilemap = gameObject.GetComponent<Tilemap>();
            tilemap.color = tilemapBoarder;
        }
        else if (gameObject.name == "Wall_Tilemap")
        {
            tilemap = gameObject.GetComponent<Tilemap>();
            tilemap.color = tilemapWallColor;
        }
        else
        {
            Debug.LogWarning("Can't find tilemap to recolorize");
        }
    }
}
