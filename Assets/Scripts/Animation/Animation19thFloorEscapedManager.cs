using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
//this class manages animation plot in escaped vampire knight route
public class Animation19thFloorEscapedManager : MonoBehaviour
{
    private GameObject objectParent;
    private GameObject vampireKnight;
    private GameObject vampirePrincess;
    private GameObject crackedFloor;

    private ParticleSystem vampireParticle;
    private ParticleSystem fog1;
    private ParticleSystem fog2;
    private ParticleSystem fog3;
    private ParticleSystem fog4;

    private Animator playerSlash1;
    private Animator playerSlash2;
    private Animator vampireSlash1;
    private Animator vampireSlash2;

    private Image blackFade;

    public static Animation19thFloorEscapedManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        objectParent = GameObject.Find("AnimationObject_Escaped");
        if (AnimationAllData.instance.is13FloorKilled || !AnimationAllData.instance.is13FloorEscaped)
        {
            Destroy(objectParent);
            return;
        }
        //invoke in escaped route
        vampireKnight = objectParent.transform.Find("vampire_knight").gameObject;
        vampirePrincess = objectParent.transform.Find("vampire_princess").gameObject;
        vampireParticle = objectParent.transform.Find("vampire_little_particle").GetComponent<ParticleSystem>();
        vampirePrincess.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        crackedFloor = GameObject.Find("Floor_Crack_Tilemap");
        crackedFloor.SetActive(false);

        playerSlash1 = GameObject.Find("Slash_Player_1").GetComponent<Animator>();
        playerSlash2 = GameObject.Find("Slash_Player_2").GetComponent<Animator>();
        vampireSlash1 = GameObject.Find("Slash_Vampire_2").GetComponent<Animator>();
        vampireSlash2 = GameObject.Find("Slash_Vampire_2").GetComponent<Animator>();

        fog1 = objectParent.transform.Find("vampire_fog_1").GetComponent<ParticleSystem>();
        fog2 = objectParent.transform.Find("vampire_fog_2").GetComponent<ParticleSystem>();
        fog3 = objectParent.transform.Find("vampire_fog_3").GetComponent<ParticleSystem>();
        fog4 = objectParent.transform.Find("vampire_fog_4").GetComponent<ParticleSystem>();

        blackFade = GameObject.Find("Black_Canvas").GetComponentInChildren<Image>();

        //no need to check null since after plot, it is going straightly to ending without pause and save point
        GameObject.Find("Killed_Plot").SetActive(false);
        GameObject.Find("Escaped_Plot").SetActive(true);
    }

    #region StartAnimation
    public void CallStartEscapedAniamtion()
    {
        StartCoroutine(StartEscapedAniamtion());
    }

    IEnumerator StartEscapedAniamtion()
    {
        float yPos = vampireKnight.transform.position.y;
        float newYPos = yPos - 0.1f;
        LeanTween.moveLocalY(vampireKnight, newYPos, 0.05f).setOnComplete(
            () =>
            {
                playerSlash1.SetTrigger("PlaySlash2");
                AudioManager.instance.Play("slash_blocked", true, 0.06f);
            }
            );
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalY(vampireKnight, yPos, 0.05f).setOnComplete(
            () =>
            {
                DialogueManager.instance.actionAfterPause = CallWinKnight;
                DialogueManager.instance.ContinueDialogueAfterPause();
            }
            );
    }

    public void CallWinKnight()
    {
        StartCoroutine(WinKnight());
    }

    IEnumerator WinKnight()
    {
        yield return new WaitForSeconds(0.5f);
        vampireSlash1.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(0.1f);
        playerSlash1.SetTrigger("PlaySlash3");
        vampireSlash1.SetTrigger("PlaySlash1");
        yield return new WaitForSeconds(0.1f);
        vampireSlash2.SetTrigger("PlaySlash4");
        vampireSlash1.SetTrigger("PlaySlash2");
        yield return new WaitForSeconds(0.9f);
        float yPos = vampireKnight.transform.position.y;
        float newYPos = yPos + 1f;
        LeanTween.moveLocalY(vampireKnight, newYPos, 0.4f);
        yield return new WaitForSeconds(0.1f);
        AudioManager.instance.Play("helmet_drop", true, 0.04f);
        GameObject vampireKnightHamlet = vampireKnight.transform.Find("vampire_head").gameObject;
        float helmetYPos = vampireKnightHamlet.transform.localPosition.y;
        LeanTween.moveLocalY(vampireKnightHamlet, helmetYPos + 0.46f, 0.5f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.rotateZ(vampireKnightHamlet, -66f, 0.6f).setEase(LeanTweenType.easeOutCubic);
        yield return new WaitForSeconds(0.6f);
        LeanTween.alpha(vampireKnightHamlet, 0f, 1f).setOnComplete(
            () =>
            {
                DialogueManager.instance.actionAfterPause = CallVampirePrincessComesOut;
                DialogueManager.instance.ContinueDialogueAfterPause();
            }
            );
    }

    //call when princess comes out
    public void CallVampirePrincessComesOut()
    {
        StartCoroutine(VampirePrincessComesOut());
    }

    IEnumerator VampirePrincessComesOut()
    {
        yield return new WaitForSeconds(0.5f);
        LeanTween.alpha(vampirePrincess, 1f, 0.7f);
        yield return new WaitForSeconds(1f);
        float yPos = vampirePrincess.transform.position.y;
        float newYPos = yPos - 3f;
        LeanTween.moveLocalY(vampirePrincess, newYPos, 0.4f);
        yield return new WaitForSeconds(0.35f);
        playerSlash1.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.actionAfterPause = CallPlayParticle;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //call when Vampire Princess going to push power
    public void CallPlayParticle()
    {
        StartCoroutine(PlayParticle());
    }

    IEnumerator PlayParticle()
    {
        yield return new WaitForSeconds(0.5f);
        vampireParticle.Play();
        AudioManager.instance.ResetPlay("horror_particle");
        yield return new WaitForSeconds(1.5f);
        fog1.Play();
        yield return new WaitForSeconds(0.2f);
        fog2.Play();
        yield return new WaitForSeconds(0.2f);
        fog3.Play();
        yield return new WaitForSeconds(0.2f);
        fog4.Play();
        yield return new WaitForSeconds(1f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_right", false);
        yield return new WaitForSeconds(0.7f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_front", false);
        yield return new WaitForSeconds(0.7f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_back", false);
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.actionAfterPause = CallFinalFight;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    //call when Vampire Princess going to be crazy
    public void CallFinalFight()
    {
        SpecialFightingSystem19th.instance.SpecialDetermineFighting("vampire_princess_angry", false);
    }

    #endregion

    #region FightWin
    //call after fight calculation is win
    public IEnumerator FightingWin()
    {
        StartCoroutine(FightBigAnimation());
        yield return new WaitForSeconds(1f);
        //fade visual
        LeanTween.alpha(blackFade.rectTransform, 1f, 2f);

        //fade audio

        AudioMixer masterMixer = Resources.Load<AudioMixer>("MasterMixer");
        StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "MasterVol", 2f, 0));

        yield return new WaitForSeconds(2f);
        //play sound effects only

        vampireParticle.Stop();
        fog1.Stop();
        fog2.Stop();
        fog3.Stop();
        fog4.Stop();
        yield return new WaitForSeconds(2f);

        AudioManager.instance.StopAll();
        StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "MasterVol", 2f, 1));
        AudioManager.instance.ResetPlay("Level19_horror");
        yield return new WaitForSeconds(1f);
        //set enviroment
        crackedFloor.SetActive(true);
        //set position
        vampirePrincess.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_princess_front_fainted", false);     
        Vector3 vampirePos = vampirePrincess.transform.position;
        vampirePos.y += 1;
        vampirePrincess.transform.position = vampirePos;

        vampireKnight.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_face_right", false);
        Vector3 knightPos = vampireKnight.transform.position;
        knightPos.x -= 1;
        vampireKnight.transform.position = knightPos;

        //fade out
        LeanTween.alpha(blackFade.rectTransform, 0f, 2f);
        yield return new WaitForSeconds(2f);
        DialogueManager.instance.actionAfterPause = CallLastFootStep;
        DialogueManager.instance.FightWin();
    }

    //call when hear footstep
    public void CallLastFootStep()
    {
        StartCoroutine(LastFootStep());
    }

    IEnumerator LastFootStep()
    {
        yield return new WaitForSeconds(0.5f);
        //hear foot step sound
        AudioManager.instance.Play("footsteps", false, 0f);
        yield return new WaitForSeconds(1f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_front", false);
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.actionAfterPause = CallTurnBackToStair;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void CallTurnBackToStair()
    {
        StartCoroutine(TurnBackToStair());

    }

    IEnumerator TurnBackToStair()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_back", false);
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.ContinueDialogueAfterPause();
    }


    //call at the last end
    public IEnumerator FinalOut()
    {
        yield return new WaitForSeconds(0.5f);
        //audio
        AudioManager.instance.SetAndFade("footsteps", 1f, 0.6f, 0f);
        //visual
        LeanTween.alpha(blackFade.rectTransform, 1f, 1f);
        yield return new WaitForSeconds(1f + 1f);
        AudioManager.instance.Stop("footsteps");

        GameManager.instance.SwitchFloor("downstairs");      
    }

    #endregion

    #region FightLose
    public IEnumerator FightingLose()
    {
        StartCoroutine(FightBigAnimation());
        yield return new WaitForSeconds(1.5f);

        DialogueManager.instance.FightLose();
    }

    #endregion

    private IEnumerator FightBigAnimation()
    {
        yield return new WaitForSeconds(1f);
        playerSlash1.SetTrigger("PlaySlash1");
        yield return new WaitForSeconds(0.1f);
        vampireSlash1.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(0.1f);
        vampireSlash2.SetTrigger("PlaySlash4");
        playerSlash2.SetTrigger("PlaySlash3");
        vampireSlash1.SetTrigger("PlaySlash4");
        playerSlash1.SetTrigger("PlaySlash2");
        playerSlash2.SetTrigger("PlaySlash4");
        vampireSlash2.SetTrigger("PlaySlash1");
        //repeat
        playerSlash1.SetTrigger("PlaySlash1");
        yield return new WaitForSeconds(0.1f);
        vampireSlash1.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(0.1f);
        vampireSlash2.SetTrigger("PlaySlash4");
        playerSlash2.SetTrigger("PlaySlash3");
        vampireSlash1.SetTrigger("PlaySlash4");
        playerSlash1.SetTrigger("PlaySlash2");
        playerSlash2.SetTrigger("PlaySlash4");
        vampireSlash2.SetTrigger("PlaySlash1");
    }
}
