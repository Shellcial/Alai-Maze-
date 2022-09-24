using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this defines all monsters and player stats, allows Monster Class to inherit
[System.Serializable]
public class PlayerStats : BaseStats
{
    private Vector3 position;
    public PlayerStats() : base()
    {
        this.position = new Vector3(0, 0, 0);
    }

    public PlayerStats(string _tileName, string _displayName, int _hp, int _atk, int _def, int _mp, Vector3 _position = new Vector3())
        : base(_tileName, _displayName, _hp, _atk, _def, _mp)
    {
        this.position = _position;
    }

    public void SetPosition(Vector3 _pos)
    {
        position = _pos;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}

    public class MonsterStats : BaseStats
    {
        private int damage;

        public MonsterStats() : base()
        {
            damage = 0;
        }
        public MonsterStats(string _tileName, string _displayName, int _hp, int _atk, int _def, int _mp)
            : base(_tileName, _displayName, _hp, _atk, _def, _mp)
        {
            damage = 0;
        }
        public int GetDamage()
        {
            return damage;
        }
        public void SetDamage(int _damage)
        {
            damage = _damage;
        }
    }

public class BaseStats
{
    private string tileName; // use to be tile name in general
    private string displayName; // use for display only
    private int hp, atk, def, mp;

    public BaseStats()
    {
        tileName = string.Empty;
        displayName = string.Empty;
        this.hp = 0;
        this.atk = 0;
        this.def = 0;
        this.mp = 0;
    }

    public BaseStats(string _tileName, string _displayName, int _hp, int _atk, int _def, int _mp)
    {
        tileName = _tileName;
        displayName = _displayName;
        this.hp = _hp;
        this.atk = _atk;
        this.def = _def;
        this.mp = _mp;
    }
    public string GetName()
    {
        return this.tileName;
    }

    public string GetDisplayName()
    {
        return this.displayName;
    }
    public int GetHP()
    {
        return this.hp;
    }
    public int GetATK()
    {
        return this.atk;
    }
    public int GetDEF()
    {
        return this.def;
    }
    public int GetMP()
    {
        return this.mp;
    }

    public string SetName(string _tileName)
    {
        return this.tileName = _tileName;
    }
    public string SetDisplayName(string _displayName)
    {
        return this.displayName = _displayName;
    }

    public int SetHP(int value)
    {
        return this.hp = value;
    }
    public int SetATK(int value)
    {
        return this.atk = value;
    }

    public int SetDEF(int modifier)
    {
        return this.def = modifier;
    }
    public int SetMP(int value)
    {
        return this.mp = value;
    }

    public int AddHP(int modifier)
    {
        return this.hp += modifier;
    }
    public int AddATK(int modifier)
    {
        return this.atk += modifier;
    }

    public int AddDEF(int modifier)
    {
        return this.def += modifier;
    }
    public int AddMP(int modifier)
    {
        return this.mp += modifier;
    }
}