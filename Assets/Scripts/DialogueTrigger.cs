using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//This class allows the interactable object to detect the hit by Player and activate Dialogue Manager
public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink Json")]
    [SerializeField]private TextAsset inkJson;

    //hit by raycast from Player (tag == Plot_Fight)
    public void OnRayCastEnter(string tileName, Tilemap tilemap, Vector3Int tilePos)
    {
        //destroy tiles
        tilemap.SetTile(tilePos, null);
        //dialogue trigger
        DialogueManager.instance.StartDialogue(inkJson);
    }

    //trigger when gameobject is collided (tag == plot_empty)
    public IEnumerator OnRayCastEnterEmptyPlot(RaycastHit2D _hit)
    {
        yield return new WaitForSeconds(0.1f);
        //send game object information to Plot statistics
        PlotStatistics.instance.UpdatePlayedPlot(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1, _hit.collider.gameObject);
        Destroy(_hit.collider.gameObject);

        DialogueManager.instance.StartDialogue(inkJson);
        //Debug.Log("Story is triggered");
    }
}
