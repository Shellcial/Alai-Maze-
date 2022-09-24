using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//this class handles the keys and door update event
public class InventoryUsage : MonoBehaviour
{
    #region Singleton
    public static InventoryUsage instance { get; private set; }
    private void Awake()
    {
        /*if (instance != null)
        {
            Debug.LogWarning("Found more than one InventoryUsage Instance");
        }*/
        instance = this;
    }
    #endregion

    private RaycastHit2D storeHit;
    private Vector3Int storteTilePos;
    public void GetKeys(string keyType)
    {
        AudioManager.instance.Play("get_item", true, 0f);
        PlayerStatistics.instance.GetKeys(keyType);
    }

    public void UseKeys(string doorType, RaycastHit2D hit, Vector3Int tilePos)
    {
        storeHit = hit;
        storteTilePos = tilePos;
        PlayerStatistics.instance.UseKeys(doorType, tilePos);
    }

    public void OpenDoor()
    {
        AudioManager.instance.Play("open_door", true, 0f);
        Tilemap temp = storeHit.collider.GetComponent<Tilemap>();
        TilemapStatistics.instance.UpdateNullTiles(temp.gameObject.name, temp.GetTile(storteTilePos), storteTilePos, UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        temp.SetTile(storteTilePos, null);
    }

}
