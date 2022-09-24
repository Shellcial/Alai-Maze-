using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;
//this class controls the final floor and ending animation
public class Animation20thFloorManager : MonoBehaviour
{
    private GameObject objectParent;
    public GameObject vampireKnight;
    public GameObject vampirePrincess;
    public GameObject candidateB;
    private GameObject player;

    private SpriteRenderer playerSprite;
    private SpriteRenderer vampireKnightSprite;
    private SpriteRenderer candidateBSprite;

    private Animator transport1;
    private Animator transport2;

    private SpriteRenderer illustrationBackground;
    private SpriteRenderer illustrationBody;

    // UI object
    private Image blackFade;

    private Image winBackground;
    private TextMeshProUGUI epilogueDisplayText;
    private TextMeshProUGUI epilogueDisplayText2;
    private TextMeshProUGUI epilogueDisplayText3;
    private bool isEpilogueEnded;
    private bool isEpilogueEnded2;

    //function to store Addressable get sprite method
    private Func<string, bool, Sprite> GetSprite;

    public static Animation20thFloorManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        objectParent = GameObject.Find("AnimationObjects");
        vampireKnight = objectParent.transform.Find("vampire_knight").gameObject;
        vampirePrincess = objectParent.transform.Find("vampire_princess").gameObject;
        player = PlayerController.instance.gameObject;
        candidateB = objectParent.transform.Find("CandidateB").gameObject;

        candidateBSprite = candidateB.GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        vampireKnightSprite = vampireKnight.GetComponent<SpriteRenderer>();

        illustrationBackground = objectParent.transform.Find("illustration_background").GetComponent<SpriteRenderer>();
        illustrationBody = illustrationBackground.transform.Find("illustration_body").GetComponent<SpriteRenderer>();
        illustrationBackground.color = new Color(0, 0, 0, 0);
        illustrationBody.color = new Color(1, 1, 1, 0);

        transport1 = GameObject.Find("Transporatation_1").GetComponent<Animator>();
        transport1.transform.position = new Vector3(-0.5f, 1.5f, 0);
        transport2 = GameObject.Find("Transporatation_2").GetComponent<Animator>();

        blackFade = GameObject.Find("Black_Canvas").GetComponentInChildren<Image>();
        blackFade.color = new Color(0, 0, 0, 1f);

        //set UI objects
        winBackground = GameObject.Find("Win_Background").GetComponent<Image>();
        winBackground.color = new Color(0f, 0f, 0f, 0f);
        epilogueDisplayText = GameObject.Find("Win_Text_1").GetComponent<TextMeshProUGUI>();
        epilogueDisplayText2 = GameObject.Find("Win_Text_2").GetComponent<TextMeshProUGUI>();
        epilogueDisplayText3 = GameObject.Find("Win_Text_3").GetComponent<TextMeshProUGUI>();
        epilogueDisplayText.alpha = 0;
        epilogueDisplayText2.alpha = 0;
        isEpilogueEnded = false;
        isEpilogueEnded2 = false;

        //set function
        GetSprite = AddressableAssets.instance.GetSprite;

        StartCoroutine(StartAnimation());
    }
    private void Update()
    {
        if (isEpilogueEnded)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                isEpilogueEnded = false;
                StartCoroutine(ToNextPage());
            }
        }
        else if (isEpilogueEnded2)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                isEpilogueEnded2 = false;
                GameManager.instance.BackToTitle();
            }
        }
    }

    #region Animation First Part
    IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(1f);
        //set sprite
        playerSprite.sprite = GetSprite("player_right", false);
        vampireKnightSprite.sprite = GetSprite("vampire_knight_face_left", false);

        LeanTween.alpha(blackFade.rectTransform, 0f, 1f);
        yield return new WaitForSeconds(1f + 1f);
        float posY = candidateB.transform.localPosition.y;
        LeanTween.moveLocalY(candidateB, posY + 2f, 2f);
        yield return new WaitForSeconds(2f + 0.5f);
        candidateBSprite.sprite = GetSprite("CandidateB_left", false);
        yield return new WaitForSeconds(1f);
        candidateBSprite.sprite = GetSprite("CandidateB_right", false);
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.actionAfterPause = CallCandidateReadyTransport;
        DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("20th_Floor_Ending"));
    }

    //call when Candidate transport
    public void CallCandidateReadyTransport()
    {
        StartCoroutine(CandidateReadyTransport());
    }

    IEnumerator CandidateReadyTransport()
    {
        yield return new WaitForSeconds(0.5f);
        candidateBSprite.sprite = GetSprite("CandidateB_back", false);
        yield return new WaitForSeconds(0.5f);
        float posY = candidateB.transform.localPosition.y;
        LeanTween.moveLocalY(candidateB, posY + 2f, 2f);
        yield return new WaitForSeconds(2f + 0.5f);
        candidateBSprite.sprite = GetSprite("CandidateB_front", false);
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.actionAfterPause = CallCandidateTransport;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //after Candidate
    public void CallCandidateTransport()
    {
        StartCoroutine(CandidateTransport());
    }

    IEnumerator CandidateTransport()
    {
        yield return new WaitForSeconds(0.5f);
        transport1.SetTrigger("PlayTransportation");
        AudioManager.instance.Play("transport", true, 0f);
        yield return new WaitForSeconds(3f);

        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(PlayerComesOut()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //after transportation, player comes out and check the condition
    public IEnumerator PlayerComesOut()
    {
        yield return new WaitForSeconds(0.5f);
        float posX = player.transform.localPosition.x;
        LeanTween.moveLocalX(player, posX + 3f, 1f);
        yield return new WaitForSeconds(1f + 0.3f);
        playerSprite.sprite = GetSprite("player_front", false);
        yield return new WaitForSeconds(1f);
        playerSprite.sprite = GetSprite("player_right", false);
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(PlayerDecideToLeave()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //player finish asking, ready to leave
    public IEnumerator PlayerDecideToLeave()
    {
        yield return new WaitForSeconds(0.5f);
        playerSprite.sprite = GetSprite("player_front", false);
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalY(player, player.transform.position.y - 1, 0.8f);
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalX(vampireKnight, vampireKnight.transform.position.x - 2, 0.5f);
        yield return new WaitForSeconds(0.1f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(PlayerTurnBack()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public IEnumerator PlayerTurnBack()
    {
        yield return new WaitForSeconds(0.2f);
        playerSprite.sprite = GetSprite("player_right", false);
        yield return new WaitForSeconds(0.4f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(HandOverGem()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }
    // improve the efficency of assigning aciton
    public IEnumerator HandOverGem()
    {
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalX(vampireKnight, vampireKnight.transform.position.x - 2f, 1f).setOnComplete(
            () =>
            {
                vampireKnightSprite.sprite = GetSprite("vampire_knight_face_front", false);
            }
            );
        yield return new WaitForSeconds(0.6f);
        playerSprite.sprite = GetSprite("player_back", false);
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(PlayerScared1()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
        UpdateUI.instance.ShowGemIcon(false);
    }

    public IEnumerator PlayerScared1()
    {
        yield return new WaitForSeconds(1f);
        LeanTween.moveLocalY(player, player.transform.position.y - 1f, 1f);
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(PlayerScared2()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public IEnumerator PlayerScared2()
    {
        yield return new WaitForSeconds(1f);
        LeanTween.moveLocalY(player, player.transform.position.y - 1f, 1f);
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalY(vampireKnight, vampireKnight.transform.position.y - 1f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(MiddleFade()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public IEnumerator MiddleFade()
    {
        yield return new WaitForSeconds(1f);
        //video
        LeanTween.alpha(blackFade.rectTransform, 1f, 1f);
        //audio
        Sound audioSource = AudioManager.instance.GetAudioSource("Level19_horror");
        if (audioSource.source.isPlaying)
        {
            AudioManager.instance.SetAndFade("Level19_horror", 1f, audioSource.source.volume, 0f);
        }

        yield return new WaitForSeconds(1f);
        //set new sprite position
        player.transform.position = new Vector3(-1.5f, 4.5f, 0);
        vampireKnight.transform.position = new Vector3(-0.5f, 4.5f, 0);
        playerSprite.sprite = GetSprite("player_right", false);
        vampireKnightSprite.sprite = GetSprite("vampire_knight_face_left", false);
        yield return new WaitForSeconds(1f);
        //viusal
        LeanTween.alpha(blackFade.rectTransform, 0f, 1f);
        //audio
        if (audioSource.source.isPlaying)
        {
            AudioManager.instance.Stop("Level19_horror");
        }
        AudioManager.instance.ResetPlay("Ending_Music");

        yield return new WaitForSeconds(1f + 1f);
        DialogueManager.instance.actionAfterPause = () => { StartCoroutine(VampireLeave()); };
        DialogueManager.instance.ContinueDialogueAfterPause();
    }
    #endregion


    #region AfterBlood
    public void ShowIllustration()
    {
        LeanTween.alpha(illustrationBackground.gameObject, 1f, 0.5f);
        LeanTween.alpha(illustrationBody.gameObject, 1f, 0.5f);
        illustrationBody.GetComponent<Animator>().SetTrigger("PlayIllustration");
    }
    public void CloseIllustration()
    {
        LeanTween.alpha(illustrationBackground.gameObject, 0f, 0.5f);
        LeanTween.alpha(illustrationBody.gameObject, 0f, 0.5f).setOnComplete(
            () => { illustrationBody.GetComponent<Animator>().SetTrigger("ResetIllustration"); }
            );
    }

    public IEnumerator VampireLeave()
    {
        transport1.gameObject.transform.position = new Vector3(2.5f, 4.5f, 0);
        transport2.gameObject.transform.position = new Vector3(3.5f, 4.5f, 0);
        yield return new WaitForSeconds(1f);
        vampireKnightSprite.sprite = GetSprite("vampire_knight_face_front", false);
        yield return new WaitForSeconds(0.5f);
        vampireKnightSprite.sprite = GetSprite("vampire_knight_face_right", false);
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalX(vampireKnight, vampireKnight.transform.position.x + 3f, 2f);
        yield return new WaitForSeconds(2f + 0.5f);
        transport1.SetTrigger("PlayTransportation");
        transport2.SetTrigger("PlayTransportation");
        AudioManager.instance.Play("transport", true, 0f);
        
        //set audio
        yield return new WaitForSeconds(1f);
        Sound audioSource = AudioManager.instance.GetAudioSource("Ending_Music");
        if (audioSource.source.isPlaying)
        {
            AudioManager.instance.SetAndFade("Ending_Music", 2f, audioSource.source.volume, 0f);
        }
        yield return new WaitForSeconds(2f);
        AudioManager.instance.ResetPlay("Level19_horror");

        yield return new WaitForSeconds(0.5f);
        //feel dizzy
        LeanTween.alpha(blackFade.rectTransform, 0.5f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        LeanTween.alpha(blackFade.rectTransform, 0f, 0.1f);
        yield return new WaitForSeconds(0.1f + 0.5f);

        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //win page1
    public IEnumerator LastSection()
    {
        //audio
        Sound audioSource = AudioManager.instance.GetAudioSource("Level19_horror");
        AudioManager.instance.SetAndFade("Level19_horror", 2f, audioSource.source.volume, 0f);
        //visual
        yield return new WaitForSeconds(1f);
        //visual
        LeanTween.alpha(winBackground.rectTransform, 1f, 3f);
        yield return new WaitForSeconds(2f);
        //audio
        AudioManager.instance.SetAndFade("Ending_Music", 2f, 0f, 0.6f);
        LeanTweenExt.LeanAlphaText(epilogueDisplayText, 1f, 3f);
        yield return new WaitForSeconds(1f);
        LeanTweenExt.LeanAlphaText(epilogueDisplayText2, 1f, 3f).setOnComplete(
            () =>
            {
                isEpilogueEnded = true;
            }
            );
    }

    //win page2
    private IEnumerator ToNextPage()
    {
        LeanTweenExt.LeanAlphaText(epilogueDisplayText, 0f, 2f);
        LeanTweenExt.LeanAlphaText(epilogueDisplayText2, 0f, 2f);
        yield return new WaitForSeconds(1.5f);
        LeanTweenExt.LeanAlphaText(epilogueDisplayText3, 1f, 2f).setOnComplete(
            () =>
            {
                isEpilogueEnded2 = true;
            }
            );
    }

    #endregion
}
