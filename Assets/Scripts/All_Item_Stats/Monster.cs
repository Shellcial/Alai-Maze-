using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//individual monster inherit stats
public class Monster
{
    public MonsterStats stats;

    public Monster()
    {
        stats = new MonsterStats();
    }
}

//list only for json file to get
public class JsonMonsters
{
    public List<JsonMonster> monsters;
}

//format for json file to assign value, this class needs exact same field with the Json file.
//so it needs be another class rather than the class inherit from stats class
[System.Serializable]
public class JsonMonster
{
    public string tileName;
    public string displayName;
    public int hp;
    public int atk;
    public int def;
    public int mp;
}
