using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey
{
    private int keyNum;
    private string keyName;
    private string doorName;

    //initiate keys
    public DoorKey()
    {
        this.keyNum = 0;
        this.keyName = string.Empty;
        this.doorName = string.Empty;
    }
    public DoorKey(int _keyNum, string _keyName, string _doorName)
    {
        this.keyNum = _keyNum;
        this.keyName = _keyName;
        this.doorName = _doorName;
    }

    public void AddKey()
    {
        this.keyNum += 1;
    }
    public void ReduceKey()
    {
        this.keyNum -= 1;
    }

    public void SetKey(int _keyNum)
    {
        this.keyNum = _keyNum;
    }

    public bool IsKeyEnough(int needKeys = 1)
    {
        return (this.keyNum >= needKeys) ? true : false;
    }

    public int GetKeyNum()
    {
        return this.keyNum;
    }
    public string GetKeyName()
    {
        return this.keyName;
    }
    public string GetDoorName()
    {
        return this.doorName;
    }
}
