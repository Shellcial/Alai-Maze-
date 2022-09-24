using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
//this class manages the save and load events, called from GameManager
//serialize file in binary format and deserialize
//attach this to GameManager for DontDestroyOnLoad
public class SaveSystem : MonoBehaviour
{
    private string saveFilePath;

    public static SaveSystem instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        string saveFolderPath = "save";
        CreateFolder(saveFolderPath);
    }

    //create folder for once only if there is none in the path
    private void CreateFolder(string _saveFolderPath)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + _saveFolderPath))
        {
            string saveFolderName = "save";
            Directory.CreateDirectory(Application.persistentDataPath + "/" + saveFolderName);
        }
    }
    public void SaveGame(GameData saveData, int saveIndex)
    {
        saveFilePath = Application.persistentDataPath + "/save" + "/save" + saveIndex + ".data";
        Debug.Log("Save Game: " + saveFilePath);
        FileStream dataStream = new FileStream(saveFilePath, FileMode.Create);
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, saveData);
        dataStream.Close();
        UpdateUI.instance.UpdateScreenSlot(saveIndex-1, saveData);
    }

    //load when start to update the savemenu
    //load when player saves progress
    public GameData LoadGame(int loadIndex)
    {
        //Debug.Log("Try to load game");
        saveFilePath = Application.persistentDataPath + "/save" + "/save" + loadIndex + ".data";
        if (File.Exists(saveFilePath))
        {
            FileStream dataStream = new FileStream(saveFilePath, FileMode.Open);

            BinaryFormatter converter = new BinaryFormatter();
            GameData saveData = converter.Deserialize(dataStream) as GameData;
            dataStream.Close();
            //Debug.Log("Load Successfully: " + saveFilePath);
            return saveData;
        }
        else
        {
            // File does not exist
            //Debug.LogWarning("Save file not found in: " + saveFilePath);
            return null;
        }
    }
}
