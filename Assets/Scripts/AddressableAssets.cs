using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;
using UnityEngine.U2D;
using System.Threading.Tasks;
using Ink.Runtime;
//this class stores player sprites 
public class AddressableAssets : MonoBehaviour
{
    
    //addressable for monster sprite
    private string spriteAtlasAddress = "AllSpriteAtlas";
    private AsyncOperationHandle<SpriteAtlas> handle;
    //[SerializeField]
    private SpriteAtlas spriteAtlas;

    //addressable for dialogue
    private AsyncOperationHandle<TextAsset> dialogueHandle;
    //[SerializeField]
    private TextAsset inkJsonFile;

    public static AddressableAssets instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        handle = Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasAddress);

        handle.Completed += _handle =>
        {
            spriteAtlas = _handle.Result;
            if (FindObjectOfType<StartLoadManager>() != null)
            {
                StartCoroutine(GameManager.instance.TryToLoadGameFromStart());
            }
            UpdateUI.instance.UpdateBookDisplay();
        };
    }

    public Sprite GetSprite(string tileName, bool is1a = true)
    {
        Sprite sprite;
        if (is1a)
        {
            sprite = spriteAtlas.GetSprite(tileName + "_1a");
        }
        else
        {
            sprite = spriteAtlas.GetSprite(tileName);
        }
        return sprite;
    }

    public void ReleaseHandle()
    {
        Addressables.Release(handle);
    }

    /*public void StartCoroutineOfGetDialogue(string dialoguePath)
    {
        StartCoroutine
    }*/
    public TextAsset GetDialogue(string dialoguePath)
    {
        //Debug.Log("start: " + dialoguePath);
        dialogueHandle = Addressables.LoadAssetAsync<TextAsset>(dialoguePath);
        return dialogueHandle.WaitForCompletion();
    }

    public void ReleaseDialogueHandle()
    {
        Addressables.Release(dialogueHandle);
    }
}
