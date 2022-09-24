using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoadManager : MonoBehaviour
{
    public bool isFileLoaded = false;
    public int loadedIndex = 0;
    public static StartLoadManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    
    public void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
    
}
