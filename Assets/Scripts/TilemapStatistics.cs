using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//this class stores all the tilemap statistics through out the game
public class TilemapStatistics : MonoBehaviour
{
    //create a list from level 1 to 20 which contain all tilemaps information
    public Dictionary<int, TilemapsInOneLevel> tilemapsInformationInAllLevels;

    public static TilemapStatistics instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        tilemapsInformationInAllLevels = new Dictionary<int, TilemapsInOneLevel>();

    }

    //pass tilemap name, target tilebase and position, current level index
    public void UpdateNullTiles(string tilemapName, TileBase tileBase, Vector3Int tilePos, int sceneIndex)
    {
        int levelIndex = sceneIndex - 1;
        if (tilemapsInformationInAllLevels.ContainsKey(levelIndex))
        {
            //current level data exists, update the current one 
            //Debug.Log("current level: " + levelIndex + " has data before");

            if (tilemapsInformationInAllLevels[levelIndex].tilemapInformation.ContainsKey(tilemapName))
            {
                //current tilemap data exists, update the current one
                //Debug.Log("current tilemap: " + tilemapName + " has data before");

                OneTileInformation newTile = new OneTileInformation(tilePos, tileBase.name);
                tilemapsInformationInAllLevels[levelIndex].tilemapInformation[tilemapName].allTiles.Add(newTile);
            }
            else
            {
                //current tilemap data does not esist, create a new one
                //Debug.Log("current tilemap " + tilemapName + "  is new");
                OneTilemapInformation newTilemapInformation = new OneTilemapInformation();
                tilemapsInformationInAllLevels[levelIndex].tilemapInformation.Add(tilemapName, newTilemapInformation);
                OneTileInformation newTile = new OneTileInformation(tilePos, tileBase.name);
                newTilemapInformation.allTiles.Add(newTile);
            }
        }
        else
        {
            //current level data does not esist, create a new one
            //Debug.Log("current level: " + levelIndex + " is new");

            TilemapsInOneLevel newLevelTilemaps = new TilemapsInOneLevel();
            tilemapsInformationInAllLevels.Add(levelIndex, newLevelTilemaps);

            OneTilemapInformation newTilemapInformation = new OneTilemapInformation();
            newLevelTilemaps.tilemapInformation.Add(tilemapName, newTilemapInformation);

            OneTileInformation newTile = new OneTileInformation(tilePos, tileBase.name);
            newTilemapInformation.allTiles.Add(newTile);
            //now a new level is spawned, inside a new tilemap information is spawned, and a tile is added. 
        }

        /*
        //test if the data stored can be accessed
        try
        {
            OneTileInformation gotTile = tilemapsInformationInAllLevels[levelIndex].tilemapInformation[tilemapName].allTiles.Find(x => x.tileName == tileBase.name);
            //Debug.Log(gotTile.tileName);
            //Debug.Log(tilemapsInformationInAllLevels);
        }
        catch
        {
            Debug.LogWarning("Cannot find tile information");
        }
        */
    }
    public void LoadTilemapData(Dictionary<int, TilemapsInOneLevel> data)
    {
        tilemapsInformationInAllLevels = data;
    }
}

[System.Serializable]
//contain each tilemap in one level
public class TilemapsInOneLevel
{
    public Dictionary<string, OneTilemapInformation> tilemapInformation;
    public TilemapsInOneLevel()
    {
        tilemapInformation = new Dictionary<string, OneTilemapInformation>();
    }
}
[System.Serializable]
//contain each tile information in one tilemap
public class OneTilemapInformation
{
    public List<OneTileInformation> allTiles;
    public OneTilemapInformation()
    {
        allTiles = new List<OneTileInformation>();
    }
}
[System.Serializable]
//contain one tile information
public class OneTileInformation
{
    public int[] tilePosition;
    public string tileName;
    public OneTileInformation(Vector3Int _tilePosition, string _tileName)
    {
        tilePosition = new int[3] { _tilePosition.x, _tilePosition.y, _tilePosition.z };
        tileName = _tileName;
    }
}