using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
//This class determines the collision and event handler between player and tilemap tiles
public class LayerCheck : MonoBehaviour
{
    #region Singleton
    public static LayerCheck instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    private Tilemap tilemap;
    private TileBase currentTile;

    private float hitPosOffset = 0.1f;

    public void CheckTag(string tagName, RaycastHit2D hit, bool leftPressed, bool downPressed)
    {
        Vector3 hitpoint = new Vector3(hit.point.x, hit.point.y, 0);
        //left press and down press need go down from .0 to .9 to get the right position 
        if (leftPressed)
        {
            hitpoint = new Vector3(hit.point.x - hitPosOffset, hit.point.y, 0);
        }
        else if (downPressed)
        {
            hitpoint = new Vector3(hit.point.x, hit.point.y - hitPosOffset, 0);
        }
        Vector3Int tilePos = Vector3Int.FloorToInt(hitpoint);
        //get the tile data
        tilemap = hit.collider.GetComponent<Tilemap>();
        currentTile = tilemap.GetTile(tilePos);
        string tileName = currentTile.name.ToString();

        //pass tilePos and tilemap for set tiles to null
        //pass tileName to determine Event
        switch (tagName)
        {
            case "Items":                
                hit.collider.GetComponent<ItemPickUp>().PickUP(tileName);
                tilemap.SetTile(tilePos, null);
                TilemapStatistics.instance.UpdateNullTiles(tilemap.gameObject.name, currentTile, tilePos, SceneManager.GetActiveScene().buildIndex);
                break;

            case "Keys":
                hit.collider.GetComponent<InventoryUsage>().GetKeys(tileName);
                tilemap.SetTile(tilePos, null);
                TilemapStatistics.instance.UpdateNullTiles(tilemap.gameObject.name, currentTile, tilePos, SceneManager.GetActiveScene().buildIndex);
                break;

            case "Doors":
                hit.collider.GetComponent<InventoryUsage>().UseKeys(tileName, hit, tilePos);
                break;

            case "Weapons":
                break;

            case "Plot_Fight":
                DialogueTrigger dialogueTrigger = null;
                dialogueTrigger = hit.collider.GetComponent<DialogueTrigger>();
                if (dialogueTrigger != null)
                {
                    dialogueTrigger.OnRayCastEnter(tileName, tilemap, tilePos);
                }
                break;

            case "Enemy":
                hit.collider.GetComponent<FightingSystem>().DetermineFight(tileName, tilemap, tilePos);
                break;

            case "Stairs":
                AudioManager.instance.Play("footstep_stairs", true, 0f);
                GameManager.instance.SwitchFloor(tileName);
                break;

            default:
                Debug.Log("layer tag not yet defined");
                break;
        }
    }
}
