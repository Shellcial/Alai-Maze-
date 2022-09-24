using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFightingSystem19th : MonoBehaviour
{
    private bool isKilled;

    public static SpecialFightingSystem19th instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SpecialDetermineFighting(string _monsterName, bool _isKilled)
    {
        isKilled = _isKilled;
        //for special boss without the needed to pass monster stats or update tile null
        //only fight on data, without visualizing the result through tilemap
        if (string.Equals(_monsterName, "vampire_knight"))
        {
            StartFighting(444, 80, 80, 100);
        }
        else if (string.Equals(_monsterName, "vampire_princess_normal"))
        {
            StartFighting(4400, 300, 50, 5000);
        }
        else if (string.Equals(_monsterName, "vampire_princess_angry"))
        {
            StartFighting(7777, 350, 50, 0);
        }
        else
        {
            Debug.LogWarning("no monster name match in special fight");
        }
    }

    void StartFighting(int _monsterHP, int _monsterATK, int _montserDEF, int _monsterMP)
    {
        int monsterDamage = _monsterATK - PlayerStatistics.instance.playerStats.GetDEF();
        int playerDamage = PlayerStatistics.instance.playerStats.GetATK() - _montserDEF;

        if (isKilled)
        {
            if (monsterDamage <= 0 && playerDamage > 0)
            {
                //directly win since player takes no damage
                StartCoroutine(Animation19thFloorKilledManager.instance.FightingWin());
                return;
            }
            else if (playerDamage <= 0)
            {
                StartCoroutine(Animation19thFloorKilledManager.instance.FightingLose());
                return;
            }
        }
        else
        {
            if (monsterDamage <= 0 && playerDamage > 0)
            {
                //directly win since player takes no damage
                StartCoroutine(Animation19thFloorEscapedManager.instance.FightingWin());
                return;
            }
            else if (playerDamage <= 0)
            {
                DialogueManager.instance.ChangeHPVariable(9999999);
                StartCoroutine(Animation19thFloorEscapedManager.instance.FightingLose());
                return;
            }
        }

        

        //calculate damage
        int takeDamage = 0;

        while (_monsterHP > 0)
        {
            _monsterHP -= playerDamage;
            if (_monsterHP > 0)
            {
                takeDamage += monsterDamage;
            }
        }
        
        //determine win loss
        if (isKilled)
        {
            if (PlayerStatistics.instance.playerStats.GetHP() > takeDamage)
            {
                PlayerStatistics.instance.AddHP(-takeDamage, true);
                PlayerStatistics.instance.AddMP(_monsterMP, true);
                StartCoroutine(Animation19thFloorKilledManager.instance.FightingWin());
            }
            else
            {
                PlayerStatistics.instance.AddHP(-takeDamage, true);
                StartCoroutine(Animation19thFloorKilledManager.instance.FightingLose());
            }
        }
        else
        {
            if (PlayerStatistics.instance.playerStats.GetHP() > takeDamage)
            {
                PlayerStatistics.instance.AddHP(-takeDamage, true);
                StartCoroutine(Animation19thFloorEscapedManager.instance.FightingWin());
            }
            else
            {
                PlayerStatistics.instance.AddHP(-takeDamage, true);
                DialogueManager.instance.ChangeHPVariable(takeDamage);
                StartCoroutine(Animation19thFloorEscapedManager.instance.FightingLose());
            }
        }
    }
}
