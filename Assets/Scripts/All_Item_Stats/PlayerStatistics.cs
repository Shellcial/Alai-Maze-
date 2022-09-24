using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
 
//this class handles all the player stats and InventoryUsage
public class PlayerStatistics : MonoBehaviour
{
    #region Player Stats Parameter
    public PlayerStats playerStats;
    private int startHP = 1000; //1000
    private int startATK = 10; // 10
    private int startDEF = 10; //10
    private int startMP = 0; //0

    #endregion
    public PersonalBelongings personalStuff = new PersonalBelongings();
    public static PlayerStatistics instance {get; private set;}
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }
    private void Start()
    {
        //initiate Player Stats
        playerStats = new PlayerStats("Player", "羅姆斯．拿塔", startHP, startATK, startDEF, startMP, PlayerController.instance.transform.localPosition);

        //initiate Key Stats
        this.personalStuff = new PersonalBelongings();

        UpdateUI.instance.deleageUpdateUI();
    }

    public void DamageTaken(int damage)
    {
        int currentHP = playerStats.GetHP();
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died.");
    }

    #region Add or Set Player Stats
    public void AddHP(int hpRecover, bool isEventStillPlaying = false)
    {
        playerStats.AddHP(hpRecover);
        UpdateUI.instance.UpdateHP();
        if (!isEventStillPlaying)
        {
            GameManager.instance.isEventPlaying = false;
        }
    }

    public void AddATK(int modifier, bool isEventStillPlaying = false)
    {
        playerStats.AddATK(modifier);
        FightingDamage.instance.UpdateMonsterDamage();
        UpdateUI.instance.UpdateATK();
        if (!isEventStillPlaying)
        {
            GameManager.instance.isEventPlaying = false;
        }
    }

    public void AddDEF(int modifier, bool isEventStillNeedToBePlaying = false)
    {
        playerStats.AddDEF(modifier);
        FightingDamage.instance.UpdateMonsterDamage();
        UpdateUI.instance.UpdateDEF();
        if (!isEventStillNeedToBePlaying)
        {
            GameManager.instance.isEventPlaying = false;
        }
    }

    public void AddMP(int modifier, bool isEventStillPlaying = false)
    {
        playerStats.AddMP(modifier);
        UpdateUI.instance.UpdateMP();
        if (!isEventStillPlaying)
        {
            GameManager.instance.isEventPlaying = false;
        }
    }
    //directly set HP (no usage at current stage)
    public void SetHP(int value)
    {
        playerStats.SetHP(value);
        UpdateUI.instance.UpdateHP();
        GameManager.instance.isEventPlaying = false;
    }

    #endregion

    #region Keys Update Event 
    public void GetKeys(string keyType)
    {
        List<DoorKey> _keyPack = this.personalStuff.KeyPack;
        foreach(DoorKey _key in _keyPack)
        {
            if (string.Equals(_key.GetKeyName(), keyType))
            {
                _key.AddKey();
            }
        }
        UpdateUI.instance.UpdateKeys();
        GameManager.instance.isEventPlaying = false;
    }

    public void UseKeys(string doorType, Vector3Int tilePos)
    {
        // Is key enough
        List<DoorKey> _keyPack = this.personalStuff.KeyPack;
        foreach (DoorKey _key in _keyPack)
        {
            if (string.Equals(_key.GetDoorName(), doorType))
            {
                if (_key.IsKeyEnough())
                {
                    _key.ReduceKey();
                    InventoryUsage.instance.OpenDoor();
                    UpdateUI.instance.UpdateKeys();
                    break;
                }
                else
                {
                    GameManager.instance.isEventPlaying = false;
                    break;
                }
            }
        }
        GameManager.instance.isEventPlaying = false;
    }

    public int GetKeyNum(string keyName1)
    {
        List<DoorKey> keyPack = this.personalStuff.KeyPack;
        foreach (DoorKey key in keyPack)
        {
            if (string.Equals(key.GetKeyName(), keyName1))
            {
                return key.GetKeyNum();
            }
        }
        return 0;
    }
    public List<int> GetKeysNum(string keyName1, string keyName2, string keyName3)
    {
        List<DoorKey> keyPack = this.personalStuff.KeyPack;
        List<int> keyNumList = new List<int>();
        foreach (DoorKey key in keyPack)
        {
            if (string.Equals(key.GetKeyName(), keyName1))
            {
                keyNumList.Add(key.GetKeyNum());
            }
            else if(string.Equals(key.GetKeyName(), keyName2))
            {
                keyNumList.Add(key.GetKeyNum());
            }
            else if(string.Equals(key.GetKeyName(), keyName3))
            {
                keyNumList.Add(key.GetKeyNum());
            }
        }
        return keyNumList;
    }
    #endregion

    //load all status when load game
    public void LoadAllStatus(int hp, int atk, int def, int mp, int yellowKeyNum, int blueKeyNum, int redKeyNum, float[] position)
    {
        playerStats.SetHP(hp);
        playerStats.SetATK(atk);
        playerStats.SetDEF(def);
        playerStats.SetMP(mp);
        personalStuff.KeyPack[0].SetKey(yellowKeyNum);
        personalStuff.KeyPack[1].SetKey(blueKeyNum);
        personalStuff.KeyPack[2].SetKey(redKeyNum);

        Vector3 playerPosition = new Vector3(position[0], position[1], position[2]);
        playerStats.SetPosition(playerPosition);

        //update player stats
        UpdateUI.instance.deleageUpdateUI();

        //update player position
        PlayerController.instance.LoadPlayerPos(playerStats.GetPosition());
    }
}
