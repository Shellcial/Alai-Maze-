using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//this class stores all data and let player to access it thorugh scene change, save and load
[System.Serializable]
public class GameData
{
    //store player status
    public int hp, atk, def, mp, yellowKeyNum, blueKeyNum, redKeyNum;
    //store date and time when saving and first loading
    public string date, time;

    //save screenshot texture
    public int textureX;
    public int textureY;
    public byte[] bytes;

    //save player position
    public float[] playerPosition;

    //save scene index (need to minus 1 to be floor index)
    public int sceneIndex;

    //a dictionary of levels which contain a dictionary of tilemap
    public Dictionary<int, TilemapsInOneLevel> tilemapsInAllLevels = new Dictionary<int, TilemapsInOneLevel>();

    //a dtictionary of levels which contain a list of plot trigger
    public Dictionary<int, MultiPlotInOneLevel> plotInAllLevels;

    //icon status
    public Dictionary<string, bool> allSwordIconBool = new Dictionary<string, bool>();
    private bool isShieldIconShowed;
    public Dictionary<string, bool> allGemIconBool = new Dictionary<string, bool>();

    //save arrived scene
    private List<int> sceneArrivedList;

    //save animation data
    public bool is13FloorAnimationEntryPlayed { get; set; }
    public bool is13FloorKilled { get; set; }
    public bool is13FloorEscaped { get; set; }

    //save plot split
    public bool isVampireKilled { get; set; }

    public GameData()
    {
        hp = 0;
        atk = 0;
        def = 0;
        mp = 0;
        yellowKeyNum = 0;
        blueKeyNum = 0;
        redKeyNum = 0;
        isShieldIconShowed = false;
        sceneArrivedList = new List<int>();
        is13FloorAnimationEntryPlayed = false;
        isVampireKilled = false;
    }

    public void SetStats(int _hp, int _atk, int _def, int _mp, int _yellowKeyNum, int _blueKeyNum, int _redKeyNum)
    {
        hp = _hp;
        atk = _atk;
        def = _def;
        mp = _mp;
        yellowKeyNum = _yellowKeyNum;
        blueKeyNum = _blueKeyNum;
        redKeyNum = _redKeyNum;
    }

    public void SetTime(string _date, string _time)
    {
        date = _date;
        time = _time;
    }
    public void SetShieldIcon()
    {
        isShieldIconShowed = true;
    }

    public bool GetShieldIcon()
    {
        return isShieldIconShowed;
    }

    public void SetSwordIcon(string iconName)
    {
        foreach (KeyValuePair<string, bool> item in allSwordIconBool.ToList())
        {
            allSwordIconBool[item.Key] = false;
        }
        allSwordIconBool[iconName] = true;
    }

    public KeyValuePair<string, bool> GetSwordTrueIcon()
    {
        KeyValuePair<string, bool> obtainedValue = new KeyValuePair<string, bool>();
        foreach (KeyValuePair<string, bool> item in allSwordIconBool.ToList())
        {
            if (item.Value == true)
            {
                obtainedValue = item;
                break;
            }
        }
        return obtainedValue;
    }

    public void SetGemIcon(string iconName)
    {
        foreach (KeyValuePair<string, bool> item in allGemIconBool.ToList())
        {
            allGemIconBool[item.Key] = false;
        }
        allGemIconBool[iconName] = true;
    }
    public KeyValuePair<string, bool> GetGemTrueIcon()
    {
        KeyValuePair<string, bool> obtainedValue = new KeyValuePair<string, bool>();
        foreach (KeyValuePair<string, bool> item in allGemIconBool.ToList())
        {
            if (item.Value == true)
            {
                obtainedValue = item;
                break;
            }
        }
        return obtainedValue;
    }

    public void SetSceneArrivedList(List<int> list)
    {
        sceneArrivedList = list;
    }

    public List<int> GetSceneArrivedList()
    {
        return sceneArrivedList;
    }
}