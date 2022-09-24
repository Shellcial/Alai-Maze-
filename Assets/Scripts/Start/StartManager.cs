using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class StartManager : MonoBehaviour
{
    //start menu
    private GameObject startMenu;
    private Button startButton;
    private Button loadButton;
    private Button aboutButton;
    private Button quitButton;

    //load menu
    private GameObject loadMenu;

    private Button saveSlotButton1;
    private Button saveSlotButton2;
    private Button saveSlotButton3;
    private Button exitSaveSlot;

    //update screen slot infomation
    public List<ScreenSlotInformation> screenSlotInformationList;
    private ScreenSlotInformation screenSlotInfo1 = new ScreenSlotInformation();
    private ScreenSlotInformation screenSlotInfo2 = new ScreenSlotInformation();
    private ScreenSlotInformation screenSlotInfo3 = new ScreenSlotInformation();
    public List<Image> screenShotImageList;
    public Image screenShotImage1;
    public Image screenShotImage2;
    public Image screenShotImage3;

    //about menu
    private GameObject aboutMenu;
    private Button aboutBackButton;

    //animation
    private GameObject blackFade;

    private TextMeshProUGUI titleText;
    private float titleTextPosY;

    private float enterFadeTime = 2f;
    private float loadFadeTime = 2f;
    private float quitFadeTime = 1f;
    private float startGameFadeTime = 2f;
    private float buttonFadeTime = 1.5f;
    private float buttonOffest = 0.15f;

    private float moveYPos = 50f;
    private List<float> endPosY;
    private List<Button> buttonLists;

    //intro
    private GameObject introObject;
    private TextMeshProUGUI introText;
    private bool isIntroEnded;
    private Image introBackground;
    private string finalText1 = "在阿萊爾軍校，";
    private string finalText2 = "為期4個月的第一學期即將完結，";
    private string finalText3 = "所有學生迎來期末考試";
    private string finalText4 = "—";
    private string finalText5 = "—";
    private string finalText6 = "在阿萊爾迷宮中取得阿萊爾迷宮之石，";
    private string finalText7 = "然後到達地下20層並使用傳送陣回到地面。";

    //audio
    private AudioMixer masterMixer;
    private float startMusicVol = 0.5f;

    void Awake()
    {
        InputSystem.DisableDevice(Mouse.current);
        InputSystem.EnableDevice(Keyboard.current);
    }

    void Start()
    {
        SetStartMenu();
        SetLoadMenu();
        SetAboutMenu();
        SetEntryAnimation();
        SetIntro();
        //audio
        masterMixer = Resources.Load<AudioMixer>("MasterMixer");

        StartCoroutine(EntryAnimation());
    }
    #region Start SetUp
    private void SetStartMenu()
    {
        startMenu = GameObject.Find("Start_Menu");
        startMenu.GetComponent<CanvasGroup>().alpha = 1;
        startMenu.SetActive(true);

        startButton = startMenu.transform.Find("Start_Button").GetComponent<Button>();
        loadButton = startMenu.transform.Find("Load_Button").GetComponent<Button>();
        aboutButton = startMenu.transform.Find("About_Button").GetComponent<Button>();
        quitButton = startMenu.transform.Find("Quit_Button").GetComponent<Button>();

        startButton.onClick.AddListener(LoadIntro);
        loadButton.onClick.AddListener(OpenLoadMenu);
        aboutButton.onClick.AddListener(OpenAboutMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void SetLoadMenu()
    {
        //load menu
        loadMenu = GameObject.Find("Load_Menu");
        loadMenu.GetComponent<CanvasGroup>().alpha = 1;
        loadMenu.SetActive(false);

        saveSlotButton1 = loadMenu.transform.Find("Save_Directory_Button1").GetComponent<Button>();
        saveSlotButton2 = loadMenu.transform.Find("Save_Directory_Button2").GetComponent<Button>();
        saveSlotButton3 = loadMenu.transform.Find("Save_Directory_Button3").GetComponent<Button>();
        exitSaveSlot = loadMenu.transform.Find("Exit_Save_Button").GetComponent<Button>();

        saveSlotButton1.onClick.AddListener(delegate { LoadGame(1); });
        saveSlotButton2.onClick.AddListener(delegate { LoadGame(2); });
        saveSlotButton3.onClick.AddListener(delegate { LoadGame(3); });
        exitSaveSlot.onClick.AddListener(CloseLoadMenu);

        screenShotImageList = new List<Image>();
        screenShotImageList.Add(loadMenu.transform.Find("Save_Directory_Info1").transform.Find("Screenshot").GetComponent<Image>());
        screenShotImageList.Add(loadMenu.transform.Find("Save_Directory_Info2").transform.Find("Screenshot").GetComponent<Image>());
        screenShotImageList.Add(loadMenu.transform.Find("Save_Directory_Info3").transform.Find("Screenshot").GetComponent<Image>());

        //time and player data
        screenSlotInformationList = new List<ScreenSlotInformation>();
        screenSlotInformationList.Add(screenSlotInfo1);
        screenSlotInformationList.Add(screenSlotInfo2);
        screenSlotInformationList.Add(screenSlotInfo3);

        for (int i = 0; i < screenSlotInformationList.Count; i++)
        {
            GameObject screenSlotInfoParent = loadMenu.transform.Find("Save_Directory_Info" + (i + 1)).gameObject;
            screenSlotInformationList[i].timeText = screenSlotInfoParent.transform.Find("Time_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].floorText = screenSlotInfoParent.transform.Find("Floor_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].hpText = screenSlotInfoParent.transform.Find("HP_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].atkText = screenSlotInfoParent.transform.Find("ATK_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].defText = screenSlotInfoParent.transform.Find("DEF_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].mpText = screenSlotInfoParent.transform.Find("MP_Text").GetComponent<TextMeshProUGUI>();
        }
        TryToLoadGameInfo();
    }

    private void SetAboutMenu()
    {
        aboutMenu = GameObject.Find("About_Menu");
        aboutMenu.GetComponent<CanvasGroup>().alpha = 1;
        aboutMenu.SetActive(false);
        aboutBackButton = aboutMenu.transform.Find("About_Quit_Button").GetComponent<Button>();
        aboutBackButton.onClick.AddListener(CloseAboutMenu);
    }
    private void SetEntryAnimation()
    {
        //animation
        //black fade
        blackFade = GameObject.Find("black_fade_background");
        Color colorTemp = blackFade.GetComponent<Image>().color;
        colorTemp.a = 1;
        blackFade.GetComponent<Image>().color = colorTemp;

        //title
        titleText = GameObject.Find("Game_Title_Text").GetComponent<TextMeshProUGUI>();
        titleText.alpha = 0f;
        titleTextPosY = titleText.gameObject.transform.localPosition.y;
        titleText.gameObject.transform.localPosition = new Vector3
                (titleText.gameObject.transform.localPosition.x,
                titleText.gameObject.transform.localPosition.y - moveYPos,
                titleText.gameObject.transform.localPosition.z);

        //button animation
        endPosY = new List<float>();
        endPosY.Add(startButton.gameObject.transform.localPosition.y);
        endPosY.Add(loadButton.gameObject.transform.localPosition.y);
        endPosY.Add(aboutButton.gameObject.transform.localPosition.y);
        endPosY.Add(quitButton.gameObject.transform.localPosition.y);

        buttonLists = new List<Button>();
        buttonLists.Add(startButton);
        buttonLists.Add(loadButton);
        buttonLists.Add(aboutButton);
        buttonLists.Add(quitButton);

        foreach (Button button in buttonLists)
        {
            //set beginning position Y
            button.gameObject.transform.localPosition = new Vector3
                (button.gameObject.transform.localPosition.x,
                button.gameObject.transform.localPosition.y - moveYPos,
                button.gameObject.transform.localPosition.z);

            //set 0 alpha
            button.GetComponentInChildren<TextMeshProUGUI>().alpha = 0;
        }
    }

    private void SetIntro()
    {
        introObject = GameObject.Find("Start_Intro");
        introObject.GetComponent<CanvasGroup>().alpha = 0;
        introText = introObject.transform.Find("Intro_Text").GetComponent<TextMeshProUGUI>();
        introBackground = introObject.transform.Find("Intro_Background").GetComponent<Image>();
        introBackground.color = new Color(0, 0, 0, 0);
        isIntroEnded = false;
    }

    #endregion

    void Update()
    {
        if (isIntroEnded)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                isIntroEnded = false;
                StartNewGame();
            }
        }
    }

    #region Animation Control
    IEnumerator EntryAnimation()
    {

        //audio
        masterMixer.SetFloat("MasterVol", -60);
        StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "MasterVol", enterFadeTime, startMusicVol));
        AudioManager.instance.Play("start_music", false, 0f);

        //visual
        LeanTween.alpha(blackFade.GetComponent<RectTransform>(), 0f, enterFadeTime).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(1.5f);

        LeanTweenExt.LeanAlphaText(titleText, 1f, buttonFadeTime);
        LeanTween.moveLocalY(titleText.gameObject, titleTextPosY, buttonFadeTime);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < endPosY.Count; i++)
        {
            LeanTweenExt.LeanAlphaText(buttonLists[i].GetComponentInChildren<TextMeshProUGUI>(), 1f, buttonFadeTime);
            if (i == endPosY.Count - 1)
            {
                LeanTween.moveLocalY(buttonLists[i].gameObject, endPosY[i], buttonFadeTime).setEase(LeanTweenType.easeOutSine).setOnComplete(
                    () =>
                    {
                        startButton.Select();
                    }
                    );
                break;
            }
            LeanTween.moveLocalY(buttonLists[i].gameObject, endPosY[i], buttonFadeTime).setEase(LeanTweenType.easeOutSine);
            yield return new WaitForSeconds(buttonOffest);
        }
    }   

    void LoadIntro()
    {
        AudioManager.instance.Play("button_start", true, 0f);

        InputSystem.DisableDevice(Keyboard.current);
        introText.SetText("");
        introObject.GetComponent<CanvasGroup>().alpha = 1;
        LeanTween.alpha(introBackground.rectTransform, 1f, 2f).setOnComplete(
            () =>
            {
                StartCoroutine(LoadIntroText());
                startMenu.SetActive(false);
                InputSystem.EnableDevice(Keyboard.current);
            }
            );
    }

    IEnumerator LoadIntroText()
    {
        string displayText = "";

        foreach (char _char in finalText1)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        isIntroEnded = true;
        foreach (char _char in finalText2)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (char _char in finalText3)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        foreach (char _char in finalText4)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (char _char in finalText5)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (char _char in finalText6)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (char _char in finalText7)
        {
            displayText += _char;
            introText.SetText(displayText);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void LoadAnimation()
    {
        //audio
        AudioManager.instance.SetAndFade("start_music", enterFadeTime, startMusicVol, 0f);

        //visual
        LeanTween.alpha(blackFade.GetComponent<RectTransform>(), 1f, loadFadeTime).setEase(LeanTweenType.easeInSine).setOnComplete(GoToGame);
    }

    #endregion

    #region UI event Control
    void OpenLoadMenu()
    {
        //audio
        ButtonOnClick();
        
        startMenu.SetActive(false);
        exitSaveSlot.Select();
        loadMenu.SetActive(true);
    }

    void CloseLoadMenu()
    {
        //audio
        ButtonOnClick();
        
        loadMenu.SetActive(false);
        loadButton.Select();
        startMenu.SetActive(true);
    }

    void OpenAboutMenu()
    {
        //audio
        ButtonOnClick(); 
        
        aboutMenu.SetActive(true);
        aboutBackButton.Select();
    }
    void CloseAboutMenu()
    {
        //audio
        ButtonOnClick();

        aboutMenu.SetActive(false);
        aboutButton.Select();
    }

    void StartNewGame()
    {
        AudioManager.instance.SetAndFade("start_music", enterFadeTime, startMusicVol, 0f);

        LeanTween.alpha(blackFade.GetComponent<RectTransform>(), 1f, startGameFadeTime).setEase(LeanTweenType.easeInSine)
            .setOnComplete(GoToGame);
    }

    void QuitGame()
    {
        InputSystem.DisableDevice(Keyboard.current);
        //audio
        AudioManager.instance.Play("button_exit", true, 0f);
        StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "MasterVol", 1f, -80));
        //visual
        LeanTween.alpha(blackFade.GetComponent<RectTransform>(), 1f, quitFadeTime).setEase(LeanTweenType.easeInSine).setOnComplete(
            () => {
                Application.Quit();
            });
    }

    void LoadGame(int loadIndex)
    {
        GameData saveData = SaveSystem.instance.LoadGame(loadIndex);
        if (saveData != null)
        {
            //audio
            AudioManager.instance.Play("button_start", true, 0f);

            StartLoadManager.instance.isFileLoaded = true;
            StartLoadManager.instance.loadedIndex = loadIndex;
            InputSystem.DisableDevice(Keyboard.current);
            LoadAnimation();
        }
        else
        {
            AudioManager.instance.Play("button_load_failed", true, 0f);
        }
    }
    void GoToGame()
    {
        SceneManager.LoadScene(1);
    }

    void ButtonOnClick()
    {
        AudioManager.instance.Play("button_general", true, 0f);
    }


    #endregion

    #region Load Game Information
    //directly copy from UpdateUI
    void TryToLoadGameInfo()
    {
        for (int i = 1; i <= screenSlotInformationList.Count; i++)
        {
            string filePath = Application.persistentDataPath + "/save/save" + i + ".data";
            if (File.Exists(filePath))
            {
                //update save slot if there is load file
                //get back game data
                GameData gameData = SaveSystem.instance.LoadGame(i);

                //apply data related to UI
                //update player status
                screenSlotInformationList[i - 1].floorText.SetText("地下" + (SceneManager.GetActiveScene().buildIndex - 1) + "層");
                screenSlotInformationList[i - 1].hpText.SetText("HP: " + gameData.hp);
                screenSlotInformationList[i - 1].atkText.SetText("ATK: " + gameData.atk);
                screenSlotInformationList[i - 1].defText.SetText("DEF: " + gameData.def);
                screenSlotInformationList[i - 1].mpText.SetText("MP: " + gameData.mp);
                screenSlotInformationList[i - 1].timeText.SetText(gameData.date + "\n" + gameData.time);

                //update screen shot
                Texture2D tex = new Texture2D(gameData.textureX, gameData.textureY);
                ImageConversion.LoadImage(tex, gameData.bytes);
                Sprite loadSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                screenShotImageList[i - 1].sprite = loadSprite;
            }
            else
            {
                CleanScreenSlot(i - 1);
            }
        }
    }

    //empty save slot if there is no load file
    private void CleanScreenSlot(int slotIndex)
    {
        Color tempColor = screenShotImageList[slotIndex].color;
        tempColor.a = 0;
        screenShotImageList[slotIndex].color = tempColor;
        screenSlotInformationList[slotIndex].floorText.SetText("");
        screenSlotInformationList[slotIndex].hpText.SetText("");
        screenSlotInformationList[slotIndex].atkText.SetText("");
        screenSlotInformationList[slotIndex].defText.SetText("");
        screenSlotInformationList[slotIndex].mpText.SetText("");
        screenSlotInformationList[slotIndex].timeText.SetText("");
    }
    #endregion

}
