using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

//this class updates UI elements, menu display 
public class UpdateUI : MonoBehaviour
{
    //UI element text
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI yKText;
    public TextMeshProUGUI bKText;
    public TextMeshProUGUI rKText;
    public TextMeshProUGUI floorText;
    //mainMenu control
    public GameObject mainMenu;
    public Button defaultSelectedButton { get; private set; }
    private Button hotkeyButton;
    private Button hotkeyBackButton;
    private Button saveButton;
    private Button loadButton;
    private Button backTitleButton;

    private GameObject hotkeyMenu;
    public GameObject saveMenu;

    private Button saveSlotButton1;
    private Button saveSlotButton2;
    private Button saveSlotButton3;
    private Button exitSaveSlot;

    public TextMeshProUGUI saveOrLoadText;

    public List<Image> screenShotImageList;
    public Image screenShotImage1;
    public Image screenShotImage2;
    public Image screenShotImage3;

    private bool isMenuOpened;

    //update screen slot infomation
    public List<ScreenSlotInformation> screenSlotInformationList;
    private ScreenSlotInformation screenSlotInfo1 = new ScreenSlotInformation();
    private ScreenSlotInformation screenSlotInfo2 = new ScreenSlotInformation();
    private ScreenSlotInformation screenSlotInfo3 = new ScreenSlotInformation();
    //true when save, false when load
    private bool isSaveMenuOpened;

    //monster information book
    //the menu
    private GameObject monsterBookMenu;
    private bool isMonsterBookOpened;

    //contain the reference single string parent object 
    [SerializeField]
    private GameObject monsterDisplayPrefab;
    //spawn all prefabs and stored in list
    [SerializeField]
    private List<GameObject> monsterDisplays = new List<GameObject>();
    //spawn inside parent
    private GameObject bookBackground;
    //spawn prefabs into one page parent
    private GameObject pageObject;
    private Func<GameObject> spawnPage;
    private int itemPerPage = 7;
    private bool isBookUpdated;

    //put all pages inside list
    [SerializeField]
    private List<GameObject> pageList = new List<GameObject>();
    private int currentPage = 0;
    //name to get each prefab child  
    private List<string> childStringObjectNames = new List<string>()
    {"Monster_Image", "Name_Text", "HP_Text","ATK_Text", "DEF_Text", "MP_Text", "Damage_Text" };
    //new spawn prefab will have y position shift
    private float monsterPostionShift = 170f;

    //store name of current level monster name without repetition
    public List<string> currentLevelMonsterNames = new List<string>();
    //store monster data and sort in ascending order
    //use above monster name to match
    List<Monster> monsterData = new List<Monster>();

    //upgrade menu
    private GameObject upgradeMenu;
    private Button hpUpgradeButton;
    private Button atkUpgradeButton;
    private Button defUpgradeButton;
    private Button backUpgradeButton;
    private bool isUpgradeMenuOpened;
    //upgrade stats
    private TextMeshProUGUI upgradeTitleText;
    private TextMeshProUGUI upgradeHPText;
    private TextMeshProUGUI upgradeATKText;
    private TextMeshProUGUI upgradeDEFText;
    private int costMP = 20;
    private int addHP = 400;
    private int addATK = 3;
    private int addDEF = 3;
    //showing item icon
    private GameObject swordIcon;
    private GameObject shieldIcon;
    private GameObject gemIcon;
    private GameObject tuskIcon;

    public bool IsShieldIconShowed;
    public Dictionary<string, bool> allSwordIconBool = new Dictionary<string, bool>();
    public const string swordIconShowedText = "SwordIconShowed";
    public const string swordIcon2ShowedText = "SwordIcon2Showed";
    public const string swordIcon3ShowedText = "SwordIcon3Showed";
    public Dictionary<string, bool> allGemIconBool = new Dictionary<string, bool>();
    public const string gemIconShowedText = "GemIconShowed";
    public const string gemIcon2ShowedText = "GemIcon2Showed";
    public const string gemIcon3ShowedText = "GemIcon3Showed";

    //flying menu
    private GameObject flyingMenu;
    private List<Button> allFloorButtons;
    private bool isFlyingMenuOpened;

    //use delegate to store all update method about UI stats
    public delegate void UpdateAll();
    public UpdateAll deleageUpdateUI;

    public static UpdateUI instance;
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
    }

    #region Start Initiation
    void Start()
    {
        InputSystem.DisableDevice(Mouse.current);
        InputSystem.EnableDevice(Keyboard.current);
        SetUIObject();
        SetMainMenu();
        SetHotKeyMenu();
        SetMonsterBook();
        SetUpgradeMenu();
        SetIcon();
        SetFlyingMenu();

        //update hp/atk/def/mp text
        deleageUpdateUI += UpdateHP;
        deleageUpdateUI += UpdateATK;
        deleageUpdateUI += UpdateDEF;
        deleageUpdateUI += UpdateMP;
        deleageUpdateUI += UpdateKeys;
        deleageUpdateUI += UpdateLevel;

        TryToLoadScreenSlotAtStart();
    }

    private void SetUIObject()
    {
        //UI stats
        hpText = GameObject.Find("HP_text").GetComponent<TextMeshProUGUI>();
        atkText = GameObject.Find("ATK_text").GetComponent<TextMeshProUGUI>();
        defText = GameObject.Find("DEF_text").GetComponent<TextMeshProUGUI>();
        mpText = GameObject.Find("MP_text").GetComponent<TextMeshProUGUI>();
        yKText = GameObject.Find("yellow_text").GetComponent<TextMeshProUGUI>();
        bKText = GameObject.Find("blue_text").GetComponent<TextMeshProUGUI>();
        rKText = GameObject.Find("red_text").GetComponent<TextMeshProUGUI>();
        floorText = GameObject.Find("Floor_text").GetComponent<TextMeshProUGUI>();
        //sword icon
        allSwordIconBool.Add(swordIconShowedText, false);
        allSwordIconBool.Add(swordIcon2ShowedText, false);
        allSwordIconBool.Add(swordIcon3ShowedText, false);
        //gem icon
        allGemIconBool.Add(gemIconShowedText, false);
        allGemIconBool.Add(gemIcon2ShowedText, false);
        allGemIconBool.Add(gemIcon3ShowedText, false);
    }

    private void SetMainMenu()
    {
        //main menu button and save menu button
        defaultSelectedButton = GameObject.Find("Continue_Button").GetComponent<Button>();
        hotkeyButton = GameObject.Find("Hotkey_Button").GetComponent<Button>();
        saveButton = GameObject.Find("Save_Button").GetComponent<Button>();
        loadButton = GameObject.Find("Load_Button").GetComponent<Button>();
        backTitleButton = GameObject.Find("Back_Button").GetComponent<Button>();
        saveSlotButton1 = GameObject.Find("Save_Directory_Button1").GetComponent<Button>();
        saveSlotButton2 = GameObject.Find("Save_Directory_Button2").GetComponent<Button>();
        saveSlotButton3 = GameObject.Find("Save_Directory_Button3").GetComponent<Button>();
        exitSaveSlot = GameObject.Find("Exit_Save_Button").GetComponent<Button>();
        saveOrLoadText = GameObject.Find("Save_Or_Load_Text").GetComponent<TextMeshProUGUI>();
        isMenuOpened = false;

        //main and save or load menu button listener
        defaultSelectedButton.onClick.AddListener(
            () =>
            {
                CloseMainMenu();
                AudioManager.instance.Play("button_general", true, 0f);
            }
            );
        hotkeyButton.onClick.AddListener(
            () =>
            {
                AudioManager.instance.Play("button_general", true, 0f);
                OpenHotkeyMenu();
            }
            );
        saveButton.onClick.AddListener(
            delegate
            {
                OpenSaveMenu(exitSaveSlot, true);
                AudioManager.instance.Play("button_general", true, 0f);
            }
            );
        loadButton.onClick.AddListener(delegate
        {
            OpenSaveMenu(exitSaveSlot, false);
            AudioManager.instance.Play("button_general", true, 0f);
        }
        );
        backTitleButton.onClick.AddListener(
            () =>
            {
                GameManager.instance.BackToTitle();
                AudioManager.instance.Play("button_exit", true, 0f);
            }
            );
        saveSlotButton1.onClick.AddListener(delegate
        {
            SaveOrLoadGame(1);
        }
        );
        saveSlotButton2.onClick.AddListener(delegate
        {
            SaveOrLoadGame(2);
        }
        );
        saveSlotButton3.onClick.AddListener(delegate
        {
            SaveOrLoadGame(3);
        }
        );
        exitSaveSlot.onClick.AddListener(
            () =>
            {
                CloseSaveMenu();
                AudioManager.instance.Play("button_general", true, 0f);
            }
            );

        //set screenSlotInformation
        //set target image
        screenShotImageList = new List<Image>();
        screenShotImageList.Add(GameObject.Find("Save_Directory_Info1").transform.Find("Screenshot").GetComponent<Image>());
        screenShotImageList.Add(GameObject.Find("Save_Directory_Info2").transform.Find("Screenshot").GetComponent<Image>());
        screenShotImageList.Add(GameObject.Find("Save_Directory_Info3").transform.Find("Screenshot").GetComponent<Image>());

        //time and player data
        screenSlotInformationList = new List<ScreenSlotInformation>();
        screenSlotInformationList.Add(screenSlotInfo1);
        screenSlotInformationList.Add(screenSlotInfo2);
        screenSlotInformationList.Add(screenSlotInfo3);

        for (int i = 0; i < screenSlotInformationList.Count; i++)
        {
            GameObject screenSlotInfoParent = GameObject.Find("Save_Directory_Info" + (i + 1));
            screenSlotInformationList[i].timeText = screenSlotInfoParent.transform.Find("Time_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].floorText = screenSlotInfoParent.transform.Find("Floor_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].hpText = screenSlotInfoParent.transform.Find("HP_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].atkText = screenSlotInfoParent.transform.Find("ATK_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].defText = screenSlotInfoParent.transform.Find("DEF_Text").GetComponent<TextMeshProUGUI>();
            screenSlotInformationList[i].mpText = screenSlotInfoParent.transform.Find("MP_Text").GetComponent<TextMeshProUGUI>();
        }

        //set and disable menu
        mainMenu = GameObject.Find("Esc_Menu");
        mainMenu.GetComponent<CanvasGroup>().alpha = 1;
        mainMenu.SetActive(false);

        saveMenu = GameObject.Find("Save_Menu");
        saveMenu.GetComponent<CanvasGroup>().alpha = 1;
        saveMenu.SetActive(false);
    }

    private void SetHotKeyMenu()
    {
        hotkeyMenu = GameObject.Find("HotKey_Menu");
        hotkeyMenu.GetComponent<CanvasGroup>().alpha = 1;
        hotkeyMenu.SetActive(false);

        hotkeyBackButton = hotkeyMenu.transform.Find("HotKey_Back_Button").GetComponent<Button>();
        hotkeyBackButton.onClick.AddListener(
            () =>
            {
                AudioManager.instance.Play("button_general", true, 0f);
                CloseHotkeyMenu();
            }
            );
    }
    private void SetMonsterBook()
    {
        //monster book menu
        monsterBookMenu = GameObject.Find("Monster_Book");
        monsterBookMenu.GetComponent<CanvasGroup>().alpha = 1;
        bookBackground = GameObject.Find("Book_Background");
        pageObject = new GameObject("page");
        Destroy(pageObject);

        //set func
        spawnPage = () => Instantiate(pageObject, pageObject.transform.localPosition, Quaternion.identity, bookBackground.transform);

        GameObject gameObject = Instantiate(pageObject, pageObject.transform.localPosition, Quaternion.identity, bookBackground.transform);
        monsterBookMenu.SetActive(false);
        isMonsterBookOpened = false;
        isBookUpdated = false;
    }

    private void SetUpgradeMenu()
    {
        //set and disable upgrade menu 
        upgradeMenu = GameObject.Find("Upgrade_Menu");
        upgradeMenu.GetComponent<CanvasGroup>().alpha = 1;
        upgradeMenu.SetActive(false);
        isUpgradeMenuOpened = false;
        //set button
        hpUpgradeButton = upgradeMenu.transform.Find("HP_Button").GetComponent<Button>();
        atkUpgradeButton = upgradeMenu.transform.Find("ATK_Button").GetComponent<Button>();
        defUpgradeButton = upgradeMenu.transform.Find("DEF_Button").GetComponent<Button>();
        backUpgradeButton = upgradeMenu.transform.Find("Back_Upgrade_Button").GetComponent<Button>();

        hpUpgradeButton.onClick.AddListener(delegate { UpgradeStats("HP"); });
        atkUpgradeButton.onClick.AddListener(delegate { UpgradeStats("ATK"); });
        defUpgradeButton.onClick.AddListener(delegate { UpgradeStats("DEF"); });
        backUpgradeButton.onClick.AddListener(
            () =>
            {
                AudioManager.instance.Play("button_general", true, 0f);
                CloseUpgradeMenu();
            }
            );

        //upgrade texts
        upgradeTitleText = upgradeMenu.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        upgradeHPText = upgradeMenu.transform.Find("HP_Button").GetComponentInChildren<TextMeshProUGUI>();
        upgradeATKText = upgradeMenu.transform.Find("ATK_Button").GetComponentInChildren<TextMeshProUGUI>();
        upgradeDEFText = upgradeMenu.transform.Find("DEF_Button").GetComponentInChildren<TextMeshProUGUI>();
    }

    private void SetIcon()
    {
        //set icon
        swordIcon = GameObject.Find("Sword_Icon");
        shieldIcon = GameObject.Find("Shield_Icon");
        gemIcon = GameObject.Find("Gem_Icon");
        tuskIcon = GameObject.Find("Tusk_Icon");
        swordIcon.SetActive(false);
        shieldIcon.SetActive(false);
        gemIcon.SetActive(false);
        tuskIcon.SetActive(false);
    }

    private void SetFlyingMenu()
    {
        flyingMenu = GameObject.Find("Flying_Menu");
        flyingMenu.GetComponent<CanvasGroup>().alpha = 1;
        flyingMenu.SetActive(false);

        allFloorButtons = new List<Button>();
        allFloorButtons = flyingMenu.transform.Find("All_Floor_Buttons").GetComponentsInChildren<Button>().ToList();

        //set 21 buttons fly to specific floor
        for (int i = 0; i < allFloorButtons.Count; i++)
        {
            int x = i;
            allFloorButtons[x].onClick.AddListener(delegate { FlyToLevel(x); });
        }

        isFlyingMenuOpened = false;
    }
    #endregion

    private void Update()
    {
        //handle open main menu and close menu through escape key
        MainMenuKeyUpdate();

        //press x to open book
        MonsterBookKeyUpdate();

        //press c to open upgrade menu
        UpgradeMenuKeyUpdate();

        //press f to open flying menu
        FlyingMenu();
    }

    #region KeyboardKeyEvent

    void MainMenuKeyUpdate()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!GameManager.instance.isEventPlaying)
            {
                if (!isMenuOpened)
                {
                    TemporaryCaptureScreen();
                }
            }
            else if (GameManager.instance.isEventPlaying && isMenuOpened)
            {
                if (saveMenu.activeSelf)
                {
                    CloseSaveMenu();
                }
                else
                {
                    CloseMainMenu();
                }
            }
        }
    }

    void MonsterBookKeyUpdate()
    {
        if (Keyboard.current.xKey.wasPressedThisFrame & isBookUpdated)
        {
            if (!GameManager.instance.isEventPlaying && !isMenuOpened)
            {
                OpenMonsterBook();
            }
            else if (isMonsterBookOpened)
            {
                CloseMonsterBook();
            }
        }

        //pass left or right arrow to turn page if exists
        if (isMonsterBookOpened && pageList.Any())
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            {
                if (currentPage + 1 < pageList.Count)
                {
                    pageList[currentPage].SetActive(false);
                    currentPage += 1;
                    pageList[currentPage].SetActive(true);
                }
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            {
                if (currentPage - 1 >= 0)
                {
                    pageList[currentPage].SetActive(false);
                    currentPage -= 1;
                    pageList[currentPage].SetActive(true);
                }
            }
        }
    }

    void UpgradeMenuKeyUpdate()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (allGemIconBool[gemIconShowedText] || allGemIconBool[gemIcon2ShowedText] || allGemIconBool[gemIcon3ShowedText])
            {
                if (!GameManager.instance.isEventPlaying && !isMenuOpened)
                {
                    OpenUpgradeMenu();
                }
                else if (isUpgradeMenuOpened)
                {
                    CloseUpgradeMenu();
                }
            }
        }
    }

    void FlyingMenu()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (allGemIconBool[gemIconShowedText] || allGemIconBool[gemIcon2ShowedText] || allGemIconBool[gemIcon3ShowedText])
            {
                if (!GameManager.instance.isEventPlaying && !isMenuOpened)
                {
                    OpenFlyingMenu();
                }
                else if (isFlyingMenuOpened)
                {
                    CloseFlyingMenu();
                }
            }
        }
    }
    #endregion

    #region Update Player Stats(lefthandside)
    public void UpdateHP()
    {
        hpText.text = PlayerStatistics.instance.playerStats.GetHP().ToString();
    }
    public void UpdateATK()
    {
        atkText.text = PlayerStatistics.instance.playerStats.GetATK().ToString();
    }
    public void UpdateDEF()
    {
        defText.text = PlayerStatistics.instance.playerStats.GetDEF().ToString();
    }
    public void UpdateMP()
    {
        mpText.text = PlayerStatistics.instance.playerStats.GetMP().ToString();
    }
    public void UpdateKeys()
    {
        yKText.text = ("x" + PlayerStatistics.instance.personalStuff.YellowKey.GetKeyNum());
        bKText.text = ("x" + PlayerStatistics.instance.personalStuff.BlueKey.GetKeyNum());
        rKText.text = ("x" + PlayerStatistics.instance.personalStuff.RedKey.GetKeyNum());
    }

    public void UpdateLevel()
    {
        int floorInt = SceneManager.GetActiveScene().buildIndex - 1;
        //minus 1 as start scene contain 1 index

        floorText.SetText("地下" + floorInt + "層");
    }
    #endregion

    #region UpdateIcon(lefthandside)
    //update shield icon
    public void ShowShieldIcon(bool isShowed)
    {
        if (isShowed)
        {
            IsShieldIconShowed = true;
            shieldIcon.SetActive(true);
        }
        else
        {
            IsShieldIconShowed = false;
            shieldIcon.SetActive(false);
        }
    }
    //update sword icon
    public void ShowSwordIcon(bool isShowed, string iconName = "")
    {
        //isShowed: when getting save data, determine showing or not showing icon
        if (isShowed & iconName != string.Empty)
        {
            foreach (KeyValuePair<string, bool> item in allSwordIconBool.ToList())
            {
                allSwordIconBool[item.Key] = false;
            }
            allSwordIconBool[iconName] = true;

            if (iconName == swordIconShowedText)
            {
                swordIcon.GetComponent<Image>().sprite = AddressableAssets.instance.GetSprite("sword", false);
            }
            else if (iconName == swordIcon2ShowedText)
            {
                swordIcon.GetComponent<Image>().sprite = AddressableAssets.instance.GetSprite("sword_half_blooded", false);
            }
            else if (iconName == swordIcon3ShowedText)
            {
                swordIcon.GetComponent<Image>().sprite = AddressableAssets.instance.GetSprite("sword_full_blooded", false);
            }
            else
            {
                Debug.LogWarning("sword icon name is not found");
            }
            swordIcon.SetActive(true);
        }
        else
        {
            swordIcon.SetActive(false);
        }
    }

    //show gem icon, also update upgrade menu state
    public void ShowGemIcon(bool isShowed, string iconName = "")
    {
        if (isShowed && iconName != string.Empty)
        {
            foreach (KeyValuePair<string, bool> item in allGemIconBool.ToList())
            {
                allGemIconBool[item.Key] = false;
            }
            allGemIconBool[iconName] = true;

            if (iconName == gemIconShowedText)
            {
                gemIcon.GetComponent<Image>().sprite = AddressableAssets.instance.GetSprite("alai_gem", false);
                ChangeUpgradeMenu(false);
            }
            else if (iconName == gemIcon2ShowedText)
            {
                gemIcon.GetComponent<Image>().sprite = AddressableAssets.instance.GetSprite("alai_gem_purple", false);
                ChangeUpgradeMenu(true);
            }
            else if (iconName == gemIcon3ShowedText)
            {
                gemIcon.GetComponent<Image>().sprite = AddressableAssets.instance.GetSprite("alai_gem_purple_pure", false);
                ChangeUpgradeMenu(true);
            }
            else
            {
                Debug.LogWarning("Gem icon is not found");
            }
            gemIcon.SetActive(true);
        }
        else
        {
            gemIcon.SetActive(false);
        }
    }

    public void ShowTuskIcon(bool isShown)
    {
        //no need to set false except from start, since the true status will continue until the end without saving chance
        if (isShown)
        {
            tuskIcon.SetActive(true);
        }
        else
        {
            tuskIcon.SetActive(false);
        }
    }
    #endregion

    #region Esc Menu
    //capture screenshot before open esc menu
    private void TemporaryCaptureScreen()
    {
        GameManager.instance.isEventPlaying = true;
        ScreenShotHandler.instance.StartScreenCapture();
    }
    public void OpenMainMenu(Button selectButton)
    {

        isMenuOpened = true;
        selectButton.Select();
        mainMenu.SetActive(true);
    }

    public void CloseMainMenu()
    {

        isMenuOpened = false;
        mainMenu.SetActive(false);
        GameManager.instance.isEventPlaying = false;
    }

    public void OpenSaveMenu(Button selectButton, bool isSave)
    {
        isSaveMenuOpened = isSave;
        if (isSave)
        {
            saveOrLoadText.SetText("儲存");
        }
        else
        {
            saveOrLoadText.SetText("載入");
        }
        mainMenu.SetActive(false);
        selectButton.Select();
        saveMenu.SetActive(true);
    }

    public void CloseSaveMenu()
    {
        saveMenu.SetActive(false);
        OpenMainMenu(defaultSelectedButton);
    }

    #endregion

    #region HotkeyMenu
    void OpenHotkeyMenu()
    {
        mainMenu.SetActive(false);
        hotkeyMenu.SetActive(true);
        hotkeyBackButton.Select();
    }

    void CloseHotkeyMenu()
    {
        hotkeyMenu.SetActive(false);
        mainMenu.SetActive(true);
        hotkeyButton.Select();
    }
    #endregion

    #region Save or Load Screen


    //update screen slot when saving in game
    public void UpdateScreenSlot(int slotIndex, GameData gameData)
    {
        Sprite screenShotSprite = ScreenShotHandler.instance.GetSavedScreenShot();
        Color tempColor = screenShotImageList[slotIndex].color;
        tempColor.a = 1;
        screenShotImageList[slotIndex].color = tempColor;
        screenShotImageList[slotIndex].sprite = screenShotSprite;
        screenShotImageList[slotIndex].preserveAspect = true;

        screenSlotInformationList[slotIndex].floorText.SetText("地下" + (SceneManager.GetActiveScene().buildIndex - 1) + "層");
        screenSlotInformationList[slotIndex].hpText.SetText("HP: " + PlayerStatistics.instance.playerStats.GetHP().ToString());
        screenSlotInformationList[slotIndex].atkText.SetText("ATK: " + PlayerStatistics.instance.playerStats.GetATK().ToString());
        screenSlotInformationList[slotIndex].defText.SetText("DEF: " + PlayerStatistics.instance.playerStats.GetDEF().ToString());
        screenSlotInformationList[slotIndex].mpText.SetText("MP: " + PlayerStatistics.instance.playerStats.GetMP().ToString());
        screenSlotInformationList[slotIndex].timeText.SetText(gameData.date + "\n" + gameData.time);
    }

    //load once only when game start
    public void TryToLoadScreenSlotAtStart()
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
    public void SaveOrLoadGame(int saveIndex)
    {
        if (isSaveMenuOpened)
        {
            AudioManager.instance.Play("button_general", true, 0f);
            GameManager.instance.SaveGame(saveIndex);
        }
        else
        {
            StartCoroutine(GameManager.instance.LoadGame(saveIndex));
        }
    }
    //reset UI opened load menu when loading into new scene
    public void LoadFinished()
    {
        saveMenu.SetActive(false);
        mainMenu.SetActive(false);
        isMenuOpened = false;
    }
    #endregion

    #region MonsterBook

    //update when first game start after monster data is updated (MonsterManager)
    //after first start, update when enter scene(level), when file is loaded (TilemapManager)
    //update when monster is killed (FightingSystem)
    public void GetCurrentLevelMonsterData(bool isStartUpdate)
    {
        //don't allow player to open book before update complete
        isBookUpdated = false;

        //?why pageObject is destroyed between scenes?
        pageObject = new GameObject("page");
        Destroy(pageObject);
        //reset current monster list
        currentLevelMonsterNames = new List<string>();

        Tilemap enemyTilemap = GameObject.Find("Enemy_Tilemap").GetComponent<Tilemap>();
        BoundsInt bound = enemyTilemap.cellBounds;
        for (int x = bound.xMin; x < bound.xMax; x++)
        {
            for (int y = bound.yMin; y < bound.yMax; y++)
            {
                TileBase tileBase = enemyTilemap.GetTile(new Vector3Int(x, y, 0));
                if (tileBase != null)
                {
                    //special treatment in 19th floor
                    if (SceneManager.GetActiveScene().buildIndex != 20)
                    {
                        currentLevelMonsterNames.Add(tileBase.name);
                    }
                    else
                    {
                        if (AnimationAllData.instance.is13FloorKilled && !AnimationAllData.instance.is13FloorEscaped)
                        {
                            //killed route
                            if (x == 6 & y == 1)
                            {
                                currentLevelMonsterNames.Add(tileBase.name);
                            }
                        }
                        else if (!AnimationAllData.instance.is13FloorKilled && AnimationAllData.instance.is13FloorEscaped)
                        {
                            //escape route
                            if (x == 6 & y == 0)
                            {
                                currentLevelMonsterNames.Add(tileBase.name);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("No scenario match");
                        }
                    }
                }
            }
            currentLevelMonsterNames = currentLevelMonsterNames.Distinct().ToList();
        }
        //store strings for displaying monster
        monsterDisplays = new List<GameObject>(currentLevelMonsterNames.Count);
        //store monster data matched with current monster and sorting order
        monsterData = new List<Monster>();

        //clear all object inside the list
        if (pageList.Any())
        {
            foreach (GameObject gameObject in pageList)
            {
                Destroy(gameObject);
            }
        }
        //respawn the first empty page
        pageList = new List<GameObject>();
        pageList.Add(spawnPage());

        //spawn specific number of string item
        for (int i = 0; i < currentLevelMonsterNames.Count; i++)
        {
            if (i > 6 && (i + itemPerPage) % itemPerPage == 0)
            {
                GameObject newPage = spawnPage();
                pageList.Add(newPage);
            }
            //spawn 7 object per page
            float newYPos = -monsterPostionShift * (i % itemPerPage);
            Vector3 spawnPos = new Vector3(0, 0, 0);
            GameObject tempObject = Instantiate(monsterDisplayPrefab, spawnPos, Quaternion.identity, bookBackground.transform);
            tempObject.transform.localPosition = monsterDisplayPrefab.transform.localPosition;
            tempObject.transform.localPosition = new Vector3(tempObject.transform.localPosition.x, newYPos, tempObject.transform.localPosition.z);
            tempObject.transform.SetParent(pageList[pageList.Count - 1].transform);
            monsterDisplays.Add(tempObject);

            //get monsterdata from name
            string monsterName = currentLevelMonsterNames[i];
            Monster monster = MonsterManager.instance.monsterList.Find(x => x.stats.GetName().Equals(monsterName));
            monsterData.Add(monster);
        }

        if (isStartUpdate)
        {
            //update monster damage when start, then update book display
            //Debug.Log("Start Update");
            FightingDamage.instance.UpdateMonsterDamage();
        }
        else
        {
            UpdateBookDisplay();
        }
        //update book display only if monster is killed

        //sorting monster data other two methods
        /*
        IOrderedEnumerable<Monster> enumMonster = from singleMonster in _monsterSortedData
                                                  orderby singleMonster.stats.GetATK()
                                                  select singleMonster;  
        */
        //_monsterSortedData.Sort((p1, p2) => p1.stats.GetATK().CompareTo(p2.stats.GetATK()));
    }

    public void UpdateBookDisplay()
    {
        //arrange monster data in asscending order base on damage
        List<Monster> _monsterSortedData = monsterData.OrderBy(x => x.stats.GetDamage()).ToList();
        for (int i = 0; i < _monsterSortedData.Count; i++)
        {
            //Debug.Log(_monsterSortedData[i].stats.GetName() + ": " + _monsterSortedData[i].stats.GetATK().ToString());
            for (int x = 0; x < childStringObjectNames.Count; x++)
            {
                string damage = _monsterSortedData[i].stats.GetDamage().ToString();
                switch (x)
                {
                    case 0:
                        Sprite sprite;
                        if (_monsterSortedData[i].stats.GetName().Contains("vampire"))
                        {
                            //special enemy does not has the same tile name pattern
                            sprite = AddressableAssets.instance.GetSprite(_monsterSortedData[i].stats.GetName(), false);
                        }
                        else
                        {

                            sprite = AddressableAssets.instance.GetSprite(_monsterSortedData[i].stats.GetName());
                        }
                        monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<Image>().sprite = sprite;
                        break;
                    case 1:
                        monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                        .SetText(_monsterSortedData[i].stats.GetDisplayName());
                        break;
                    case 2:
                        monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                        .SetText(_monsterSortedData[i].stats.GetHP().ToString());
                        break;
                    case 3:
                        monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                        .SetText(_monsterSortedData[i].stats.GetATK().ToString());
                        break;
                    case 4:
                        monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                        .SetText(_monsterSortedData[i].stats.GetDEF().ToString());
                        break;
                    case 5:
                        monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                        .SetText(_monsterSortedData[i].stats.GetMP().ToString());
                        break;
                    case 6:
                        if (!damage.Equals(FightingDamage.limitlessDamage.ToString()))
                        {
                            monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                            .SetText(damage);
                        }
                        else
                        {
                            monsterDisplays[i].transform.Find(childStringObjectNames[x]).gameObject.GetComponent<TextMeshProUGUI>()
                            .SetText("無限");
                        }
                        break;
                    default:
                        Debug.LogWarning("Can't find index match with x (number of monster stats).");
                        break;
                }
            }
        }
        isBookUpdated = true;
    }
    private void OpenMonsterBook()
    {

        GameManager.instance.isEventPlaying = true;
        for (int i = 0; i < pageList.Count; i++)
        {
            if (i == 0)
            {
                pageList[i].SetActive(true);
                continue;
            }
            pageList[i].SetActive(false);
        }
        monsterBookMenu.SetActive(true);
        currentPage = 0;
        isMonsterBookOpened = true;
    }

    private void CloseMonsterBook()
    {
        currentPage = 0;
        monsterBookMenu.SetActive(false);
        GameManager.instance.isEventPlaying = false;
        isMonsterBookOpened = false;
    }
    #endregion

    #region Upgrade Menu

    private void OpenUpgradeMenu()
    {
        GameManager.instance.isEventPlaying = true;
        hpUpgradeButton.Select();
        upgradeMenu.SetActive(true);
        isUpgradeMenuOpened = true;
    }

    public void ChangeUpgradeMenu(bool isGem2Showed)
    {
        if (isGem2Showed)
        {
            costMP = 60;
            addHP = 1500;
            addATK = 10;
            addDEF = 10;
        }
        else
        {
            costMP = 20;
            addHP = 400;
            addATK = 3;
            addDEF = 3;
        }
        upgradeTitleText.SetText("花費" + costMP + "魔力");
        upgradeHPText.SetText("+" + addHP + " HP");
        upgradeATKText.SetText("+" + addATK + " ATK");
        upgradeDEFText.SetText("+" + addDEF + " DEF");
    }

    private void UpgradeStats(string upgradeStat)
    {
        if (PlayerStatistics.instance.playerStats.GetMP() >= costMP)
        {
            //audio
            AudioManager.instance.Play("button_upgrade_floor", true, 0f);

            PlayerStatistics.instance.AddMP(-costMP, true);
            switch (upgradeStat)
            {
                case "HP":
                    PlayerStatistics.instance.AddHP(addHP, true);
                    break;
                case "ATK":
                    PlayerStatistics.instance.AddATK(addATK, true);
                    break;
                case "DEF":
                    PlayerStatistics.instance.AddDEF(addDEF, true);
                    break;
                default:
                    Debug.LogWarning("Can't find upgrade string");
                    break;
            }
        }
        else
        {
            //aduio
            AudioManager.instance.Play("button_load_failed", true, 0f);
        }
    }

    private void CloseUpgradeMenu()
    {
        GameManager.instance.isEventPlaying = false;
        upgradeMenu.SetActive(false);
        isUpgradeMenuOpened = false;
    }
    #endregion

    #region Flying Menu
    private void OpenFlyingMenu()
    {
        GameManager.instance.isEventPlaying = true;
        isFlyingMenuOpened = true;
        allFloorButtons[SceneManager.GetActiveScene().buildIndex - 1].Select();
        flyingMenu.SetActive(true);
    }
    private void CloseFlyingMenu(bool isFlying = false)
    {
        flyingMenu.SetActive(false);
        isFlyingMenuOpened = false;
        if (!isFlying)
        {
            GameManager.instance.isEventPlaying = false;
        }
    }

    private void FlyToLevel(int levelIndex)
    {
        //audio
        AudioManager.instance.Play("button_upgrade_floor", true, 0f);

        CloseFlyingMenu(true);
        GameManager.instance.isLoadedByFile = false;
        int indexDifference = levelIndex + 1 - SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.FlyFloor(indexDifference);
    }

    public void UpdateEnableButton(List<int> sceneIndexList)
    {
        for (int i = 0; i < allFloorButtons.Count; i++)
        {
            if (sceneIndexList.Contains(i + 1))
            {
                if (!allFloorButtons[i].enabled || i == 0)
                {
                    EnableButton(i);
                }
                continue;
            }
            allFloorButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = 0.5f;
            allFloorButtons[i].enabled = false;
            allFloorButtons[i].gameObject.GetComponent<Image>().enabled = false;
        }
    }

    private void EnableButton(int i)
    {
        allFloorButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
        allFloorButtons[i].enabled = true;
        allFloorButtons[i].gameObject.GetComponent<Image>().enabled = true;
    }
    #endregion
}

public class ScreenSlotInformation
{
    public string testFont;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI mpText;
}