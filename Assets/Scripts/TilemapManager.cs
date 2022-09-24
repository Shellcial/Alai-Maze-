using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//this class store each tilemap of one level
//this class also store if player enter level
public class TilemapManager : MonoBehaviour
{
    private Dictionary<string, Tilemap> allTilemaps;

    private const string itemTilemapName = "Items_Tilemap";
    private const string keyTilemapName = "Keys_Tilemap";
    private const string doorTilemapName = "Door_Tilemap";
    private const string enemyTilemapName = "Enemy_Tilemap";
    private const string specialItemTilemapName = "Special_Items_Tilemap";
    public List<string> allTilemapNames;


    public static TilemapManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        allTilemapNames = new List<string>() { itemTilemapName, keyTilemapName, doorTilemapName, enemyTilemapName, specialItemTilemapName };
        allTilemaps = new Dictionary<string, Tilemap>();
        //set 4 tilemaps reference and add into dictionary
        for (int i = 0; i < allTilemapNames.Count; i++)
        {
            if (GameObject.Find(allTilemapNames[i]) != null)
            {
                //only add tilemap which current level has
                allTilemaps.Add(allTilemapNames[i], GameObject.Find(allTilemapNames[i]).GetComponent<Tilemap>());
            }
        }
        SetTilemapTiles();

        //for the 1st time start, get current level monster data is called from monster manager
        //for the 2nd time (with stairs switch or loading game data, call get current level monster data from here
        if (!GameManager.instance.isFirstLoaded)
        {
            UpdateUI.instance.GetCurrentLevelMonsterData(true);
        }

        int currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        List<int> sceneArrivedList = GameManager.instance.sceneArrivedList;
        if (!sceneArrivedList.Contains(currentLevelIndex))
        {
            sceneArrivedList.Add(currentLevelIndex);
        }
        UpdateUI.instance.UpdateEnableButton(sceneArrivedList);
    }

    //reset tilemap for every entry of level
    public void SetTilemapTiles()
    {
        //set every tilemap's tile in current level
        int levelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;
        Dictionary<int, TilemapsInOneLevel> _tilemapsInformationInAllLevels = TilemapStatistics.instance.tilemapsInformationInAllLevels;

        if (_tilemapsInformationInAllLevels.ContainsKey(levelIndex))
        {
            //level has updated before
            for (int i = 0; i < allTilemapNames.Count; i++)
            {
                //check all tilemaps in current level
                if (_tilemapsInformationInAllLevels[levelIndex].tilemapInformation.TryGetValue(allTilemapNames[i], out OneTilemapInformation currentTilemapInOneLevel))
                {
                    //current tilemap has update before
                    if (currentTilemapInOneLevel.allTiles != null)
                    {
                        //current tilemap has updated(nulled) tile
                        foreach (OneTileInformation tileInfo in currentTilemapInOneLevel.allTiles)
                        {
                            //null tile
                            //Debug.Log(tileInfo.tilePosition[0]+","+tileInfo.tilePosition[1] + "," + tileInfo.tilePosition[2]);
                            //Debug.Log(allTilemaps[allTilemapNames[i]].gameObject.name);
                            Vector3Int tempInt = new Vector3Int(tileInfo.tilePosition[0], tileInfo.tilePosition[1], tileInfo.tilePosition[2]);
                            allTilemaps[allTilemapNames[i]].SetTile(tempInt, null);
                        }
                    }
                }
                else
                {
                    //Debug.Log(allTilemapNames[i] + " does not have any null tiles update");
                }
            }
        }
        else
        {
            //Debug.Log("whole level does not have any update");
        }
    }
}
