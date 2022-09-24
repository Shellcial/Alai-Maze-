using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public class AnimationAllData : MonoBehaviour
{
    public bool is13FloorAnimationEntryPlayed;
    public bool is13FloorKilled;
    public bool is13FloorEscaped;

    public static AnimationAllData instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        is13FloorAnimationEntryPlayed = false;
        is13FloorKilled = false;
        is13FloorEscaped = false;
    }
}
