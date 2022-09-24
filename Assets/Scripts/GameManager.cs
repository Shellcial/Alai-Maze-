using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
//this class manage the basic game event
//switch floor, call save and load methods
public class GameManager : MonoBehaviour
{
    //load scene
    private int currentSceneIndex;

    //scene arrived data
    public List<int> sceneArrivedList = new List<int>();

    //player go up and down position
    [SerializeField]
    private TextAsset playerUpstairsJson;
    [SerializeField]
    private TextAsset playerDownstairsJson;

    private AllPlayerUpstairsPositions allUpPosition;
    private AllPlayerDownstairsPositions allDownPosition;

    private bool isPlayerGoDown = true;

    //events: collision checking, collision event: dialogue playing, fighting, item picking up, menu status ,etc.
    public bool isEventPlaying;

    //save and load system
    private GameData saveData;
    public bool isLoadedByFile;
    public bool isFirstLoaded;

    //time data
    private string date;
    private string time;

    //instantiate plot manager
    [SerializeField] private PlotManager plotManagerPrefab;
    [SerializeField] private DialogueManager dialogueManagerPrefab;
    [SerializeField] private TilemapManager tilemapManagerPrefab;

    //animation playing boolean
    public bool isAnimationPlaying;

    //plot split
    public bool isVampireKilled = false;

    //black fade in fade out
    private Image blackFade;

    //audio
    private AudioMixer masterMixer;

    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        allUpPosition = JsonUtility.FromJson<AllPlayerUpstairsPositions>(playerUpstairsJson.text);
        allDownPosition = JsonUtility.FromJson<AllPlayerDownstairsPositions>(playerDownstairsJson.text);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        isEventPlaying = true;
        isPlayerGoDown = true;
        isLoadedByFile = false;
        isFirstLoaded = true;
        isAnimationPlaying = true;
    }

    private void Start()
    {
        masterMixer = Resources.Load<AudioMixer>("MasterMixer");
    }

    public void SwitchFloor(string tileName)
    {
        isLoadedByFile = false;
        if (tileName == "upstairs")
        {
            LoadPreviousScene();
        }
        else if (tileName == "downstairs")
        {
            LoadNextScene();
        }
        else
        {
            Debug.LogWarning("stairs name does not match");
        }
    }

    public void FlyFloor(int indexDifference)
    {
        if (indexDifference > 0)
        {
            LoadNextScene(indexDifference);
        }
        else
        {
            //go to previous or current scene if loaded data is lower or at current floor
            LoadPreviousScene(-indexDifference);
        }
    }

    private void LoadNextScene(int index = 1)
    {
        currentSceneIndex += index;
        isPlayerGoDown = true;
        StartCoroutine(WaitForSceneLoaded(currentSceneIndex));
    }
    private void LoadPreviousScene(int index = 1)
    {
        currentSceneIndex -= index;
        if (index == 0)
        {
            isPlayerGoDown = true;
        }
        else
        {
            isPlayerGoDown = false;
        }
        StartCoroutine(WaitForSceneLoaded(currentSceneIndex));
    }
    IEnumerator WaitForSceneLoaded(int sceneIndex)
    {
        isFirstLoaded = false;

        if (FindObjectOfType<StartLoadManager>() == null)
        {
            //fade out level when it is not loaded from start page
            //visual
            blackFade = GameObject.Find("Black_Canvas").GetComponentInChildren<Image>();
            LeanTween.alpha(blackFade.rectTransform, 1f, 0.5f);

            //aduio
            //fade out
            StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "MusicVol", 0.5f, 0));

            yield return new WaitForSeconds(0.5f);
        }

        SceneManager.LoadScene(currentSceneIndex);
        while (SceneManager.GetActiveScene().buildIndex != sceneIndex)
        {
            yield return null;
        }
        //don't allow instant move during fade at level start
        isAnimationPlaying = true;
        ContinueLoadedAction();
    }
    //handle the start method for don't destroy on load object
    private void ContinueLoadedAction()
    {
        UpdateUI.instance.LoadFinished();
        if (!isLoadedByFile)
        {
            //reset player position when go upstairs or downstairs
            //Player is not destroyed so it won't update at start
            //it should be updated here
            PlayerController.instance.UpdatePlayerPosBewteenScene();
        }

        //can't set camera render when loading same folder slot from the second time
        CanvasCamera.instance.SetCanvasRenderCamera();

        UpdateUI.instance.deleageUpdateUI();

        //reset player alpha (mainly for level 13 scenario)
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
    }

    public Vector2 ReturnPlayerStartPos()
    {
        Vector2 vector2 = new Vector2();
        if (isPlayerGoDown)
        {
            vector2 = new Vector2(allDownPosition.allPlayerDownstairsPosition[currentSceneIndex - 1].xPos, allDownPosition.allPlayerDownstairsPosition[currentSceneIndex - 1].yPos);
        }
        else
        {
            vector2 = new Vector2(allUpPosition.allPlayerUpstairsPosition[currentSceneIndex - 1].xPos, allUpPosition.allPlayerUpstairsPosition[currentSceneIndex - 1].yPos);
        }
        return vector2;
    }

    public void SaveGame(int saveIndex)
    {
        saveData = new GameData();

        //set player stats
        PlayerStats _playerStats = PlayerStatistics.instance.playerStats;
        List<DoorKey> _doorKey = PlayerStatistics.instance.personalStuff.KeyPack;
        List<int> keysNum = PlayerStatistics.instance.GetKeysNum
            (
                    PlayerStatistics.instance.personalStuff.KeyPack[0].GetKeyName(),
                    PlayerStatistics.instance.personalStuff.KeyPack[1].GetKeyName(),
                    PlayerStatistics.instance.personalStuff.KeyPack[2].GetKeyName()
            );
        saveData.SetStats
            (
            _playerStats.GetHP(), _playerStats.GetATK(), _playerStats.GetDEF(), _playerStats.GetMP(),
            keysNum[0], keysNum[1], keysNum[2]
            );
        Vector3 playerPositionVector3 = _playerStats.GetPosition();
        float[] playerPosition = new float[] { playerPositionVector3.x, playerPositionVector3.y, playerPositionVector3.z };
        saveData.playerPosition = playerPosition;

        //set date and time
        date = System.DateTime.Now.ToString("yyyy-MM-dd");
        time = System.DateTime.Now.ToString("HH:mm:ss");
        saveData.SetTime(date, time);

        //set screenshot texture
        Texture2D tex = ScreenShotHandler.instance.screenShotTexture;
        saveData.textureX = tex.width;
        saveData.textureY = tex.height;
        saveData.bytes = ImageConversion.EncodeToPNG(tex);

        //set floor index
        saveData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        //set tilemap information
        saveData.tilemapsInAllLevels = TilemapStatistics.instance.tilemapsInformationInAllLevels;

        //set plot information
        saveData.plotInAllLevels = PlotStatistics.instance.plotInAllLevel;

        //set Icon bool state
        //shield
        if (UpdateUI.instance.IsShieldIconShowed)
        {
            saveData.SetShieldIcon();
        }
        //sword
        saveData.allSwordIconBool = UpdateUI.instance.allSwordIconBool;
        foreach (KeyValuePair<string, bool> item in saveData.allSwordIconBool.ToList())
        {
            if (item.Value == true)
            {
                saveData.SetSwordIcon(item.Key);
            }
        }
        //gem
        saveData.allGemIconBool = UpdateUI.instance.allGemIconBool;
        foreach (KeyValuePair<string, bool> item in saveData.allGemIconBool.ToList())
        {
            if (item.Value == true)
            {
                saveData.SetGemIcon(item.Key);
            }
        }

        //set arrived scene list
        saveData.SetSceneArrivedList(sceneArrivedList);

        //set animation data
        saveData.is13FloorAnimationEntryPlayed = AnimationAllData.instance.is13FloorAnimationEntryPlayed;
        saveData.is13FloorKilled = AnimationAllData.instance.is13FloorKilled;
        saveData.is13FloorEscaped = AnimationAllData.instance.is13FloorEscaped;

        //pass game save data
        SaveSystem.instance.SaveGame(saveData, saveIndex);
    }

    public IEnumerator LoadGame(int loadIndex)
    {
        saveData = null;
        saveData = SaveSystem.instance.LoadGame(loadIndex);
        if (saveData != null)
        {
            if (FindObjectOfType<StartLoadManager>() == null)
            {
                //audio
                AudioManager.instance.Play("button_start", true, 0f);

                blackFade = GameObject.Find("Black_Canvas").GetComponentInChildren<Image>();
                LeanTween.alpha(blackFade.rectTransform, 1f, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }

            //get back save data and apply to game object
            //get player stats
            PlayerStatistics.instance.LoadAllStatus(
                saveData.hp, saveData.atk, saveData.def, saveData.mp,
                saveData.yellowKeyNum, saveData.blueKeyNum, saveData.redKeyNum, saveData.playerPosition
                );
            //player position is changed base on save data, not the default position
            isLoadedByFile = true;

            //get tilemap data
            TilemapStatistics.instance.tilemapsInformationInAllLevels = saveData.tilemapsInAllLevels;

            //get plot data
            PlotStatistics.instance.plotInAllLevel = saveData.plotInAllLevels;

            //get icon data
            UpdateUI.instance.allSwordIconBool = saveData.allSwordIconBool;
            //shield
            if (saveData.GetShieldIcon())
            {
                UpdateUI.instance.ShowShieldIcon(true);
            }
            else
            {
                UpdateUI.instance.ShowShieldIcon(false);
            }
            //sword
            if (saveData.GetSwordTrueIcon().Key == null)
            {
                UpdateUI.instance.ShowSwordIcon(false);
            }
            else
            {
                UpdateUI.instance.ShowSwordIcon(true, saveData.GetSwordTrueIcon().Key);
            }
            //gem
            UpdateUI.instance.allGemIconBool = saveData.allGemIconBool;
            if (saveData.GetGemTrueIcon().Key == null)
            {
                UpdateUI.instance.ShowGemIcon(false);
            }
            else
            {
                UpdateUI.instance.ShowGemIcon(true, saveData.GetGemTrueIcon().Key);
            }

            //get scene arrived list
            sceneArrivedList = saveData.GetSceneArrivedList();

            //get animation data
            AnimationAllData.instance.is13FloorAnimationEntryPlayed = saveData.is13FloorAnimationEntryPlayed;
            AnimationAllData.instance.is13FloorKilled = saveData.is13FloorKilled;
            AnimationAllData.instance.is13FloorEscaped = saveData.is13FloorEscaped;

            //compare save floor to current floor
            int indexDifference = saveData.sceneIndex - SceneManager.GetActiveScene().buildIndex;

            FlyFloor(indexDifference);
        }
        else
        {
            AudioManager.instance.Play("button_load_failed", true, 0f);
        }
    }

    public void BackToTitle()
    {
        //audio
        StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "MasterVol", 1f, 0));

        //visual
        blackFade = GameObject.Find("Black_Canvas").GetComponentInChildren<Image>();
        LeanTween.alpha(blackFade.rectTransform, 1f, 1f).setOnComplete(
            () =>
            {
                GameObject emptyParentObject = new GameObject();
                GameObject[] objectList = FindObjectsOfType<GameObject>();
                foreach (GameObject gameObject in objectList)
                {
                    if (gameObject.name != "~LeanTween" && gameObject.name != "Black_Canvas" && gameObject.name != "Black_Canvas_Fade")
                    {
                        gameObject.transform.SetParent(emptyParentObject.transform);
                    }
                }
                //AddressableAssets.instance.ReleaseHandle();
                SceneManager.LoadScene(0);
            });
    }

    public IEnumerator TryToLoadGameFromStart()
    {
        yield return new WaitForSeconds(0f);
        int loadIndex = StartLoadManager.instance.loadedIndex;
        if (StartLoadManager.instance.isFileLoaded && loadIndex != 0)
        {
            StartLoadManager.instance.DestroyThisObject();
            StartCoroutine(LoadGame(loadIndex));
        }
        else
        {
            StartLoadManager.instance.DestroyThisObject();
        }
    }

    public void AnimationEnded()
    {
        isAnimationPlaying = false;
        isEventPlaying = false;
    }
}

#region  PlayerPosition
//stores all default player position from json data 
[System.Serializable]
public class PlayerUpstairsPosition
{
    public string level;
    public float xPos;
    public float yPos;
}

[System.Serializable]
public class PlayerDownstairsPosition
{
    public string level;
    public float xPos;
    public float yPos;
}

public class AllPlayerUpstairsPositions
{
    public List<PlayerUpstairsPosition> allPlayerUpstairsPosition;
}

public class AllPlayerDownstairsPositions
{
    public List<PlayerDownstairsPosition> allPlayerDownstairsPosition;
}
#endregion