using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
//this class manages animation plot in killed vampire knight route
public class Animation19thFloorKilledManager : MonoBehaviour
{
    private GameObject objectParent;
    private GameObject vampirePrincess;
    private SpriteRenderer vampireWings;
    private ParticleSystem vampireParticle;

    private Animator playerSlash1;
    private Animator playerSlash2;
    private Animator vampireSlash1;
    private Animator vampireSlash2;
    private Animator transportPlayer;

    private Image winBackground;
    private TextMeshProUGUI epilogueDisplayText;
    private TextMeshProUGUI epilogueDisplayText2;
    private bool isEpilogueEnded;

    public static Animation19thFloorKilledManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        objectParent = GameObject.Find("AnimationObject_Killed");
        if (!AnimationAllData.instance.is13FloorKilled || AnimationAllData.instance.is13FloorEscaped)
        {
            Destroy(objectParent);
            return;
        }
        //invoke in killed route

        vampirePrincess = objectParent.transform.Find("vampire_princess").gameObject;
        vampireParticle = objectParent.transform.Find("vampire_little_particle").GetComponent<ParticleSystem>();
        vampireWings = vampirePrincess.transform.Find("vampire_wings").GetComponent<SpriteRenderer>();
        vampireWings.color = new Color(1f, 1f, 1f, 0f);

        playerSlash1 = GameObject.Find("Slash_Player_1").GetComponent<Animator>();
        playerSlash2 = GameObject.Find("Slash_Player_2").GetComponent<Animator>();
        vampireSlash1 = GameObject.Find("Slash_Vampire_2").GetComponent<Animator>();
        vampireSlash2 = GameObject.Find("Slash_Vampire_2").GetComponent<Animator>();

        transportPlayer = GameObject.Find("Transporatation").GetComponent<Animator>();

        //no need to check null since after plot, it is going straightly to ending without pause and save point
        GameObject.Find("Killed_Plot").SetActive(true);
        GameObject.Find("Escaped_Plot").SetActive(false);

        //set UI objects
        winBackground = GameObject.Find("Win_Background").GetComponent<Image>();
        winBackground.color = new Color(0f, 0f, 0f, 0f);
        epilogueDisplayText = GameObject.Find("Win_Text_1").GetComponent<TextMeshProUGUI>();
        epilogueDisplayText2 = GameObject.Find("Win_Text_2").GetComponent<TextMeshProUGUI>();
        epilogueDisplayText.alpha = 0;
        epilogueDisplayText2.alpha = 0;
        isEpilogueEnded = false;

    }

    private void Update()
    {
        if (isEpilogueEnded)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                isEpilogueEnded = false;
                GameManager.instance.BackToTitle();
            }
        }
    }

    #region StartKilledAnimation
    //call before vampire turn around
    public void CallTurnGirlFront()
    {
        StartCoroutine(TurnGirlFront());
    }

    IEnumerator TurnGirlFront()
    {
        yield return new WaitForSeconds(1f);
        vampirePrincess.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_princess_front_whole_normal", false);
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.actionAfterPause = CallVampireGoFront;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }
    // when vampire go front
    public void CallVampireGoFront()
    {
        StartCoroutine(VampireStepFront());
    }

    IEnumerator VampireStepFront()
    {
        yield return new WaitForSeconds(0.2f);
        float yPos = vampirePrincess.transform.localPosition.y;
        float newYPos = yPos - 1;
        LeanTween.moveLocalY(vampirePrincess, newYPos, 0.2f);
        yield return new WaitForSeconds(0.1f);
        DialogueManager.instance.actionAfterPause = CallChangeFace;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //call when vampire princess is going to use power
    public void CallChangeFace()
    {
        StartCoroutine(ChangeFace());
    }

    IEnumerator ChangeFace()
    {
        yield return new WaitForSeconds(1f);
        vampireParticle.Play();
        yield return new WaitForSeconds(2f);
        LeanTween.alpha(vampireWings.gameObject, 1f, 2f);
        yield return new WaitForSeconds(2f);
        DialogueManager.instance.actionAfterPause = CallStartFighting;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    #endregion

    #region KilledFighting
    //call when starting fight
    public void CallStartFighting()
    {
        StartCoroutine(StartFighting());
    }

    IEnumerator StartFighting()
    {
        yield return new WaitForSeconds(1f);
        playerSlash1.SetTrigger("PlaySlash1");
        yield return new WaitForSeconds(0.2f);
        vampireSlash1.SetTrigger("PlaySlash2");
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<SpecialFightingSystem19th>().SpecialDetermineFighting("vampire_princess_normal", true);
    }
    #endregion

    #region Fingting Win
    public IEnumerator FightingWin()
    {
        vampireSlash1.SetTrigger("PlaySlash4");
        vampireSlash2.SetTrigger("PlaySlash3");
        vampireSlash1.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(1.5f);
        vampireParticle.Stop();
        DialogueManager.instance.actionAfterPause = CallVampireDied;
        DialogueManager.instance.FightWin();
    }

    //call when vampire dies
    public void CallVampireDied()
    {
        StartCoroutine(VampireDied()); 
    }

    IEnumerator VampireDied()
    {
        yield return new WaitForSeconds(0.5f);
        LeanTween.alpha(GameObject.Find("vampire_princess"), 0f, 2f);
        yield return new WaitForSeconds(2f +0.5f);
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public IEnumerator PlayerWinGoBack()
    {
        //turn back and play particle
        yield return new WaitForSeconds(1f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_front", false);
        yield return new WaitForSeconds(1f);
        Vector3 particlePos = vampireParticle.gameObject.transform.position;
        particlePos.y -= 1;
        vampireParticle.gameObject.transform.position = particlePos;
        vampireParticle.Play();
        yield return new WaitForSeconds(3f);
        transportPlayer.SetTrigger("PlayTransportation");
        AudioManager.instance.Play("transport", true, 0f);
        yield return new WaitForSeconds(0.5f);
        vampireParticle.Stop();
        yield return new WaitForSeconds(3f);
        StartCoroutine(EndingEpilogue());
    }
    private IEnumerator EndingEpilogue()
    {
        LeanTween.alpha(winBackground.rectTransform, 1f, 3f);
        yield return new WaitForSeconds(2f);
        LeanTweenExt.LeanAlphaText(epilogueDisplayText, 1f, 3f);
        yield return new WaitForSeconds(1f);
        LeanTweenExt.LeanAlphaText(epilogueDisplayText2, 1f, 3f);
        yield return new WaitForSeconds(3f);
        isEpilogueEnded = true;
    }
    #endregion

    #region Fighting Lose
    public IEnumerator FightingLose()
    {
        playerSlash1.SetTrigger("PlaySlash4");
        playerSlash2.SetTrigger("PlaySlash3");
        playerSlash1.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(0.5f);
        Sound audioSource = AudioManager.instance.GetAudioSource("Level19_horror");
        if (audioSource.source.isPlaying)
        {
            AudioManager.instance.SetAndFade(audioSource.name, 2f, audioSource.source.volume, 0f);
        }
        DialogueManager.instance.FightLose();
    }

    //call after lose dialogue end
    public IEnumerator AfterFightingLose()
    {
        Sound audioSource = AudioManager.instance.GetAudioSource("Level19_horror");
        if (audioSource.source.isPlaying)
        {
            AudioManager.instance.SetAndFade(audioSource.name, 2f, audioSource.source.volume, 0f);
        }
        yield return new WaitForSeconds(1f);
        LeanTween.alpha(PlayerController.instance.gameObject, 0f, 1f);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LoseUIManager.instance.ShowLoseUI());
    }
    #endregion

}