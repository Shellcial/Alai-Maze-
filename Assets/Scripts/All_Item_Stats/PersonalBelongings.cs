using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class stores item: key
//and handle key and door events

//this class stores all personal items
public class PersonalBelongings
{
    public DoorKey YellowKey;
    public DoorKey BlueKey;
    public DoorKey RedKey;

    private int startYellowKey = 0;
    private int startBlueKey = 0;
    private int startRedKey = 0;

    public List<DoorKey> KeyPack;
    public PersonalBelongings()
    {
        this.YellowKey = new DoorKey(startYellowKey, "yellow_key", "yellow_door");
        this.BlueKey = new DoorKey(startBlueKey, "blue_key", "blue_door");
        this.RedKey = new DoorKey(startRedKey, "red_key", "red_door");

        this.KeyPack = new List<DoorKey>();
        this.KeyPack.Add(this.YellowKey);
        this.KeyPack.Add(this.BlueKey);
        this.KeyPack.Add(this.RedKey);
    }
}
