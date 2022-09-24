using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
//this class controls the dailogue ink system
//singleton structure
public class DialogueManager : MonoBehaviour
{
    private GameObject dialoguePanel;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI dialogueSpeaker;
    private Image speakerImage;

    private Story currentStory;
    private bool isDialogueStarted;
    private bool isDialoguePlaying;

    private string playerName = "拿塔";
    private string narratorName = "";
    private string candidateAName = "考生A";
    private string candidateBName = "考生B";
    private string vampireKnightName = "骷髏騎士？";
    private string vampireKnightIdentity = "吸血鬼騎士";
    private string vampireKnightTrueName = "艾";
    private string littleGirl = "小女孩";
    private string vampireLittleGirl = "吸血鬼小女孩";
    private string vampirePrincess = "吸血鬼公主";
    private string unknownName = "？？？";

    private float waitCharTime = 0.05f;

    //instant display dialogue
    private string tempText;
    private Coroutine storedCoroutine;

    //do event before starting dialogue
    private string variableEvent = "specialEvent";

    //pause function during dialogue
    private string tagPause = "Pause_Dialogue";
    private bool isDialogueToBePaused;
    public Action actionAfterPause;
    public static DialogueManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        dialoguePanel = GameObject.Find("Dialogue_Panel");
        dialogueText = dialoguePanel.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        dialogueSpeaker = dialoguePanel.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        speakerImage = dialoguePanel.transform.Find("Image").GetComponent<Image>();

        dialoguePanel.GetComponent<CanvasGroup>().alpha = 1;
        dialoguePanel.SetActive(false);
        dialogueText.SetText("");
        isDialogueStarted = false;
        isDialoguePlaying = false;
        isDialogueToBePaused = false;
    }

    private void Update()
    {
        if (!GameManager.instance.isEventPlaying || !isDialogueStarted)
        {
            // allow continue dialogue when isEventPlaying is true and isDialogueStarted is true
            return;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isDialoguePlaying)
            {
                // instant show all texts if dialogue is already playing
                StopCoroutine(storedCoroutine);
                dialogueText.SetText(tempText);
                isDialoguePlaying = false;
            }
            else if (isDialogueToBePaused)
            {
                //aduio
                //AudioManager.instance.Play("dialogue_play", true, 0f);
                //if dialogue is going to be paused, pause it
                PauseDialogue();
            }
            else
            {
                //AudioManager.instance.Play("dialogue_play", true, 0f);
                ContinueDailogue();
            }
        }
    }
    public void StartDialogue(TextAsset inkJSON)
    {
        GameManager.instance.isEventPlaying = true;
        currentStory = new Story(inkJSON.text);

        if (currentStory.variablesState[variableEvent] != null && !isDialogueStarted)
        {
            //has event before the start of dialogue 
            switch (currentStory.variablesState[variableEvent])
            {
                case "SwordFightingSound":
                    AudioManager.instance.Play("fighting_sowrd", true, 0f);
                    StartCoroutine(WaitDailogue(1.5f));
                    break;
                case "FightingVampireKnight1":
                    StartCoroutine(Animation13thFloorManager.instance.StartSurpriseAttackAnimation());
                    break;
                case "SeeGirl":
                    actionAfterPause = Animation19thFloorKilledManager.instance.CallTurnGirlFront;
                    StartCoroutine(WaitDailogue(1f));
                    break;
                case "SeeKnight":
                    actionAfterPause = Animation19thFloorEscapedManager.instance.CallStartEscapedAniamtion;          
                    StartCoroutine(WaitDailogue(0.5f));
                    break;
                default:
                    Debug.Log("No variable name match");
                    break;
            }
        }
        else
        {
            isDialogueStarted = true;
            ContinueDailogue();
        }
    }


    private IEnumerator WaitDailogue(float _time)
    {
        float i = 0f;
        while (i < _time)
        {
            i += Time.deltaTime;
            yield return null;
        }
        isDialogueStarted = true;
        ContinueDailogue();
    }

    private void ContinueDailogue()
    {
        if (currentStory.canContinue)
        {
            tempText = currentStory.Continue();
            //do tag action
            if (currentStory.currentTags.Count > 0)
            {
                //set speaker name
                //name tag should be set in the first tag manually
                speakerImage.gameObject.SetActive(true);
                switch (currentStory.currentTags[0])
                {
                    case "Player":
                        dialogueSpeaker.SetText(playerName);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("player_front", false);
                        break;
                    case "Narrator":
                        dialogueSpeaker.SetText(narratorName);
                        speakerImage.gameObject.SetActive(false);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("", false);
                        break;
                    case "Vampire_Knight":
                        dialogueSpeaker.SetText(vampireKnightName);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_knight_front", false);
                        break;
                    case "Little_Girl_Back":
                        dialogueSpeaker.SetText(littleGirl);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_princess_back", false);
                        break;
                    case "Little_Girl":
                        dialogueSpeaker.SetText(littleGirl);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_princess_front_whole_normal", false);
                        break;
                    case "Vampire_Girl":
                        dialogueSpeaker.SetText(vampireLittleGirl);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_princess_front_whole_vampire", false);
                        break;
                    case "Vampire_Knight_Identity":
                        dialogueSpeaker.SetText(vampireKnightIdentity);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_knight_face_front", false);
                        break;
                    case "Vampire_Knight_True_Name":
                        dialogueSpeaker.SetText(vampireKnightTrueName);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_knight_face_front", false);
                        break;
                    case "Vampire_Princess":
                        dialogueSpeaker.SetText(vampirePrincess);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("vampire_princess_front_whole_normal", false);
                        break;
                    case "CandidateA":
                        dialogueSpeaker.SetText(candidateAName);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("CandidateA_front", false);
                        break;
                    case "CandidateB":
                        dialogueSpeaker.SetText(candidateBName);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("CandidateB_front", false);
                        break;
                    case "Unknown":
                        dialogueSpeaker.SetText(unknownName);
                        speakerImage.gameObject.SetActive(false);
                        speakerImage.sprite = AddressableAssets.instance.GetSprite("", false);
                        break;
                }

                //other tag action
                //not the best method, this repeats the above check tag method
                if (currentStory.currentTags.Contains("Add100MP"))
                {
                    PlayerStatistics.instance.AddMP(100, true);
                }
                if (currentStory.currentTags.Contains("Sub500HP"))
                {
                    PlayerStatistics.instance.AddHP(-500, true);
                    if (PlayerStatistics.instance.playerStats.GetHP() <= 0)
                    {
                        ChangeHPVariable(500);
                        FightLose();
                        return;
                    }
                }
                if (currentStory.currentTags.Contains("AddItem"))
                {
                    UpdateUI.instance.ShowTuskIcon(true);
                }
                if (currentStory.currentTags.Contains("Illustration1"))
                {
                    Animation20thFloorManager.instance.ShowIllustration();
                }
                if (currentStory.currentTags.Contains("EndIllustration1"))
                {
                    Animation20thFloorManager.instance.CloseIllustration();
                }
                if (currentStory.currentTags.Contains(tagPause))
                {
                    isDialogueToBePaused = true;
                }
            }
            //speaker name remain the last name if no tag
            dialoguePanel.SetActive(true);
            isDialoguePlaying = true;
            storedCoroutine = StartCoroutine(LoadDialogueText(tempText));
        }
        else
        {
            EndDailogue();
        }
    }

    private IEnumerator LoadDialogueText(string _text)
    {
        dialogueText.text = "";
        foreach (char _char in _text)
        {
            dialogueText.text += _char;
            yield return new WaitForSeconds(waitCharTime);
        }
        isDialoguePlaying = false;
    }

    private void EndDailogue()
    {
        //check is ink has end event after end dialogue
        if (currentStory.currentTags.Contains("End"))
        {
            EndDailogueDisplay();
            foreach (string tag in currentStory.currentTags)
            {
                switch (tag)
                {
                    case "SwordFightingSound":
                        StartCoroutine(Animation12thFloorManager.instance.Animation12thFloorDownstairs());
                        break;
                    case "FightingVampireKnight1":
                        StartCoroutine(Animation13thFloorManager.instance.StartDealAnimation());
                        break;
                    case "End13Animation":
                        Animation13thFloorManager.instance.EndAnimation();
                        break;
                    case "Escape_End":
                        StartCoroutine(Animation13thFloorManager.instance.EscapeAnimation());
                        break;
                    case "Escape_End_Vampire":
                        StartCoroutine(Animation13thFloorManager.instance.EndEscapeAnimation());
                        break;
                    case "SurpriseLose":
                        StartCoroutine(Animation13thFloorManager.instance.SurpriseLose());
                        break;
                    case "NormalLose":
                        StartCoroutine(Animation13thFloorManager.instance.FightingLose());
                        break;
                    case "LastLose":
                        StartCoroutine(Animation19thFloorKilledManager.instance.AfterFightingLose());
                        break;
                    case "WinGoBack":
                        StartCoroutine(Animation19thFloorKilledManager.instance.PlayerWinGoBack());
                        break;
                    case "FinalWin":
                        StartCoroutine(Animation19thFloorEscapedManager.instance.FinalOut());
                        break;
                    case "WholeGameLast":
                        StartCoroutine(Animation20thFloorManager.instance.LastSection());
                        break;

                }
            }
        }
        else
        {
            //end dialogue normally
            GameManager.instance.isEventPlaying = false;
            EndDailogueDisplay();
        }
    }

    private void EndDailogueDisplay()
    {
        isDialogueStarted = false;
        dialoguePanel.SetActive(false);
        dialogueText.SetText("");
    }

    private void PauseDialogue()
    {
        isDialogueStarted = false;
        dialoguePanel.SetActive(false);
        //call certain function after pause dialogue(should be defined before starting this dialogue)
        actionAfterPause();
    }

    public void ContinueDialogueAfterPause()
    {
        dialoguePanel.SetActive(true);
        isDialogueStarted = true;
        GameManager.instance.isEventPlaying = true;
        isDialogueToBePaused = false;
        ContinueDailogue();
    }

    public void FightWin()
    {
        currentStory.ChoosePathString("Win");
        ContinueDialogueAfterPause();
    }

    public void FightLose()
    {
        currentStory.ChoosePathString("Lose");
        ContinueDialogueAfterPause();
    }

    public void ChangeHPVariable(int hpDamage)
    {
        currentStory.variablesState["damage"] = hpDamage;
    }
}
