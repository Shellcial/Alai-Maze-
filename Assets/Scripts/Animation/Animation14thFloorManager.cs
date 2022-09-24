using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
//this class control 14th floor gem tile display
//another attempt is changing tilemap display after addressable sprite is loaded, while this is not done after start instantly.
//To prevent player sees the changes, change must be made at start and thus sprite should be displayed adready in start stage
//-->make two tilemap, set null to another one base on boolean value
public class Animation14thFloorManager : MonoBehaviour
{
    private Tilemap targetTilemap;
    private Vector3Int tilePos = new Vector3Int(-4, -4, 0);
    int sceneIndex;
    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateGemTile();
    }

    //update after addressable sprite is finished
    private void UpdateGemTile()
    {
        if (AnimationAllData.instance.is13FloorKilled)
        {
            //set normal gem to be null
            targetTilemap = GameObject.Find("Items_Tilemap").GetComponent<Tilemap>();
            //TileBase tileBase = targetTilemap.GetTile(tilePos);
            //TilemapStatistics.instance.UpdateNullTiles(targetTilemap.gameObject.name, tileBase, tilePos, sceneIndex);
            targetTilemap.SetTile(tilePos, null);
        }
        else
        {
            //set poluted gem to be null
            targetTilemap = GameObject.Find("Special_Items_Tilemap").GetComponent<Tilemap>();
            //TileBase tileBase = targetTilemap.GetTile(tilePos);
            //TilemapStatistics.instance.UpdateNullTiles(targetTilemap.gameObject.name, tileBase, tilePos, sceneIndex);
            targetTilemap.SetTile(tilePos, null);
        }
    }

}
