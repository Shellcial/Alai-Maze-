using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class pre-calculates the damage that player will take from monster
public class FightingDamage : MonoBehaviour
{
    public const int limitlessDamage = 999999999;

    private PlayerStats playerStats;
    private MonsterStats monsterStats;

    private int monsterHP;

    private string currentTileName;

    public static FightingDamage instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //calculate after player stats change: atk or def change because of upgrading or getting equipment
    public void UpdateMonsterDamage()
    {
        //Calculate damage after each 
        foreach (string name in UpdateUI.instance.currentLevelMonsterNames)
        {
            CalculateDamage(name);
        }
        //update the UI
        UpdateUI.instance.UpdateBookDisplay();
    }

    //calculate damage of each monster in current level
    public void CalculateDamage(string _tileName)
    {
        currentTileName = _tileName;
        playerStats = PlayerStatistics.instance.playerStats;
        //match monster
        monsterStats = MonsterManager.instance.GetMonsterStats(currentTileName);
        if (monsterStats != null)
        {
            if (playerStats.GetATK() > monsterStats.GetDEF())
            {
                SimulateFight();
            }
            else
            {
                PlayerTakesLimitlessDamage();
            }
        }
        else
        {
            Debug.LogWarning("No Monster matches");
            GameManager.instance.isEventPlaying = false;
        }
    }
    void SimulateFight()
    {
        monsterHP = monsterStats.GetHP();
        if (monsterStats.GetATK() <= playerStats.GetDEF())
        {
            //check if monster has direct attack

            //wins without losing HP
            PlayerTakesNoDamage();
        }
        else
        {
            //fighting calculation
            int playerDamage = 0;
            while (monsterHP > 0)
            {
                monsterHP -= playerStats.GetATK() - monsterStats.GetDEF();
                if (monsterHP > 0)
                {
                    playerDamage = playerDamage + monsterStats.GetATK() - playerStats.GetDEF();
                }
                else
                {
                     PlayerTakesDamage(playerDamage);
                }
            }
        }
    }

    void PlayerTakesNoDamage()
    {
        MonsterManager.instance.GetMonsterStats(currentTileName).SetDamage(0);
    }

    void PlayerTakesDamage(int _damage)
    {
        MonsterManager.instance.GetMonsterStats(currentTileName).SetDamage(_damage);
    }
    void PlayerTakesLimitlessDamage()
    {
        //Debug.Log("Fight will lose");
        MonsterManager.instance.GetMonsterStats(currentTileName).SetDamage(limitlessDamage);
    }
}