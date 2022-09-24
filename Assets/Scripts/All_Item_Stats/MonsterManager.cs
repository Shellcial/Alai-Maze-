using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//This class stores all the data of Monster
//
public class MonsterManager : MonoBehaviour
{
    //get monster data json file
    //private TextAsset monsterDataJsonFile;
    [SerializeField]
    private AssetReferenceT<TextAsset> monsterDataJsonFile;

    public bool isReferenceGot;

    //generate list with individual monster
    [SerializeField]
    public List<Monster> monsterList = new List<Monster>();

    public static MonsterManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        isReferenceGot = false;
        //get monster data json file
        monsterDataJsonFile.LoadAssetAsync().Completed += SetMonsterData;
        //monsterDataJsonFile = Resources.Load<TextAsset>("MonsterData/MonsterDataJson");
    }

    private void SetMonsterData(AsyncOperationHandle<TextAsset> obj)
    {
        //instantiate all monster data from json file
        JsonMonsters monstersJson = JsonUtility.FromJson<JsonMonsters>(obj.Result.text);
        int index = 0;
        foreach (JsonMonster _monster in monstersJson.monsters)
        {
            Monster monster = new Monster();
            monsterList.Add(monster);
            monsterList[index].stats = new MonsterStats(
                _monster.tileName, _monster.displayName ,_monster.hp, _monster.atk, _monster.def, _monster.mp);
            index++;
        }
        monsterDataJsonFile.ReleaseAsset();
        isReferenceGot = true;

        //first time update monster data from monster manager
        UpdateUI.instance.GetCurrentLevelMonsterData(true);
    }

    public MonsterStats GetMonsterStats(string _tileName)
    {
        try
        {
            return monsterList.Find(x => x.stats.GetName() == _tileName).stats;
        }
        catch
        {
            Debug.LogWarning("Can't return monster stats: " + _tileName);
            return null;
        }
    }

    public Monster GetMonster(string _tileName)
    {
        try
        {
            return monsterList.Find(x => x.stats.GetName() == _tileName);
        }
        catch
        {
            Debug.LogWarning("Can't return monster: " + _tileName);
            return null;
        }
    }
}
