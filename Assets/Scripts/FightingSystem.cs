using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class FightingSystem : MonoBehaviour
{
    private Tilemap tilemap;
    private Vector3Int tilePos;

    //store every monster damage towards player
    public Dictionary<string, int> monsterDamages = new Dictionary<string, int>();

    public void DetermineFight(string _tileName, Tilemap _tilemap, Vector3Int _tilePos)
    {
        tilemap = _tilemap;
        tilePos = _tilePos;
        MonsterStats monsterStats = MonsterManager.instance.GetMonsterStats(_tileName);
        int damage = monsterStats.GetDamage();
        int playerHp = PlayerStatistics.instance.playerStats.GetHP();

        if (damage != FightingDamage.limitlessDamage && playerHp > damage)
        {
            PlayerStatistics.instance.AddHP(-damage, true);
            PlayerStatistics.instance.AddMP(monsterStats.GetMP(), true);
           
            if (string.Equals(_tileName, "vampire_knight_front"))
            {
                //special monster fight event
                StartCoroutine(Animation13thFloorManager.instance.FightingWin());
                FightingWin(true);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 11 && string.Equals(_tileName, "skeleton_knight"))
            {
                //10 level skeleton monster fight
                AudioManager.instance.Play("slash2", true, 0.031f);
                FightingWin(true);
                DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("10th_Floor_After_Boss"));
            }
            else
            {
                //general monster fight event
                FightingWin();
            }
        }
        else
        {
            if (string.Equals(_tileName, "vampire_knight_front"))
            {
                //special monster fight event
                DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("13th_Floor_Normal_Lose"));
            }
            else
            {
                //general monster fight event
                FightLose();
            }
        }
    }

    void FightingWin(bool isSpecialFight = false)
    {

        //destroy monster, update tile data
        //Debug.Log("fight won");
        TilemapStatistics.instance.UpdateNullTiles(tilemap.gameObject.name, tilemap.GetTile(tilePos), tilePos, UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        tilemap.SetTile(tilePos, null);
        UpdateUI.instance.GetCurrentLevelMonsterData(false);
        if (!isSpecialFight)
        {
            AudioManager.instance.Play("slash2", true, 0.031f);
            GameManager.instance.isEventPlaying = false;
        }
    }

    void FightLose(bool isSpecialFight = false)
    {
        //nothing changes
        if (!isSpecialFight)
        {
            AudioManager.instance.Play("fight_fail", true, 0f);
            GameManager.instance.isEventPlaying = false;
        }
    }

}
