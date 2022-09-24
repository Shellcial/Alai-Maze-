using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
//this class handles the animation which does not allow player control
public class Animation13thFloorManager : MonoBehaviour
{

    //Fighting Entry Animation
    private GameObject fightingAnimation;
    private GameObject vampireKnight;
    private GameObject vampireKnightSecondStart;
    public GameObject candidateA { get; private set; }
    public GameObject candidateB { get; private set; }

    private GameObject vomitBloodStatic;

    private Animator slashVampireAnimator1;
    private Animator slashCandidateAAnimator1;
    private Animator slashCandidateAAnimator2;
    private Animator slashCandidateBAnimator1;
    private Animator slashCandidateBAnimator2;
    private Animator transportationA;
    private Animator transportationB;
    private Animator vomitBloodAnimate;

    private float attackMoveTime = 0.1f;
    private float attackOffset = 0.1f;
    private float attackMoveY = 0.2f;
    private float retreatMoveTime = 0.4f;
    private float retreatMoveY = 1f;

    private float playerEndPosX = -2.5f;
    private float playerEscapeEndPosX = 3.5f;

    //player fight animation
    private Animator surpriseSlash;
    private Animator playerSlash;

    public static Animation13thFloorManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        fightingAnimation = GameObject.Find("Fighting_Animation");
        vampireKnight = GameObject.Find("Vampire_Knight");
        vampireKnightSecondStart = GameObject.Find("Vampire_Knight_Second_Start");
        vomitBloodAnimate = GameObject.Find("Vomit_Blood_Animate").GetComponent<Animator>();
        vomitBloodStatic = GameObject.Find("Vomit_Blood_Static");

        if (AnimationAllData.instance.is13FloorKilled || AnimationAllData.instance.is13FloorEscaped)
        {
            //all animation is played
            Destroy(fightingAnimation);
            vomitBloodStatic.SetActive(true);
            vampireKnight.SetActive(false);
            vampireKnightSecondStart.SetActive(false);
            if (AnimationAllData.instance.is13FloorEscaped)
            {
                //don't trigger fight again when arriving the point
                GameObject.Find("Fight_Plot_Trigger").SetActive(false);
                //use for development stage
                /*if (GameObject.Find("Escape_Plot_Trigger"))
                {
                    GameObject.Find("Escape_Plot_Trigger").SetActive(false);
                }
                */
            }
            else
            {
                //don't trigger escape again when arriving the point
                GameObject.Find("Escape_Plot_Trigger").SetActive(false);
                //use for development stage
                /*if (GameObject.Find("Fight_Plot_Trigger"))
                {
                    GameObject.Find("Fight_Plot_Trigger").SetActive(false);
                }*/
            }
            //use for testing, normally tilemap statistics has stored this tile data after nomral process to the end of plot
            //while testing in development stage, player might not kill and invoke the set null method in escape method
            //DestroyVampireTile();
        }
        else if (AnimationAllData.instance.is13FloorAnimationEntryPlayed)
        {
            //entry animation is played, while still not attack/escape
            Destroy(fightingAnimation);
            vomitBloodStatic.SetActive(true);
            vampireKnight.SetActive(false);
            SetFightingObject();
            return;
        }
        else
        {
            //play entry animation
            GameManager.instance.isEventPlaying = true;
            SetEntryObjectAtStart();
            StartCoroutine(Animation13thFloorEnterStart());
        }
    }

    private void SetEntryObjectAtStart()
    {
        //only use in entry animation
        //after entry, enter this level will only call set fighting object
        vampireKnightSecondStart.SetActive(false);
        vomitBloodStatic.SetActive(false);
        candidateA = fightingAnimation.transform.Find("Orange_Person").gameObject;
        candidateB = fightingAnimation.transform.Find("Green_Person").gameObject;
        slashVampireAnimator1 = fightingAnimation.transform.Find("Slash_Vimpare_1").GetComponent<Animator>();
        slashCandidateAAnimator1 = fightingAnimation.transform.Find("Slash_CandidateA_1").GetComponent<Animator>();
        slashCandidateAAnimator2 = fightingAnimation.transform.Find("Slash_CandidateA_2").GetComponent<Animator>();
        slashCandidateBAnimator1 = fightingAnimation.transform.Find("Slash_CandidateB_1").GetComponent<Animator>();
        slashCandidateBAnimator2 = fightingAnimation.transform.Find("Slash_CandidateB_2").GetComponent<Animator>();
        transportationA = fightingAnimation.transform.Find("Transporatation_A").GetComponent<Animator>();
        transportationB = fightingAnimation.transform.Find("Transporatation_B").GetComponent<Animator>();
        SetFightingObject();
    }

    private void SetFightingObject()
    {
        surpriseSlash = GameObject.Find("Vampire_Surprise_Slash").GetComponent<Animator>();
        playerSlash = GameObject.Find("Player_Duel_Slash").GetComponent<Animator>();
    }

    #region Entry Part Animation
    IEnumerator Animation13thFloorEnterStart()
    {
        yield return new WaitForSeconds(1f);
        //can't set true too early since set canvas alpha will get this value at start
        AnimationAllData.instance.is13FloorAnimationEntryPlayed = true;
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_right", false);

        //Debug.Log("13th Floor Aniamtion playing");
        //let player finish the move first 
        yield return new WaitForSeconds(0.5f);

        PlayerMove();
    }

    void PlayerMove()
    {
        LeanTween.moveLocalX(PlayerController.instance.gameObject, playerEndPosX, 2f).setEase(LeanTweenType.easeOutSine).setOnComplete(
            () =>
            {
                PlayerController.instance.StorePlayerPos(true);
                PlayerController.instance.updatePos = PlayerController.instance.gameObject.transform.position;
                //start dialogue
                DialogueManager.instance.actionAfterPause = CallCandidateFight;
                DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("13th_Floor_Enter"));
            }
        );
    }

    //call after dialogue pause
    public void CallCandidateFight()
    {
        StartCoroutine(CandidateFight());
    }

    private IEnumerator CandidateFight()
    {
        yield return new WaitForSeconds(0.5f);
        float startYPos = candidateB.transform.position.y;
        float newYPos = startYPos + attackMoveY;

        //green person moves and attack
        LeanTween.moveLocalY(candidateB, newYPos, attackMoveTime).setOnComplete(
            () =>
            {
                slashVampireAnimator1.SetTrigger("PlaySlash4");
            }
        );
        //orange person moves and attack
        yield return new WaitForSeconds(0.3f);
        LeanTween.moveLocalY(candidateA, newYPos, attackMoveTime).setOnComplete(
            () =>
            {
                slashVampireAnimator1.SetTrigger("PlaySlash5");
            }
        );
        //attack again
        yield return new WaitForSeconds(attackMoveTime + attackOffset);
        slashVampireAnimator1.SetTrigger("PlaySlash2");
        //people moves back
        yield return new WaitForSeconds(0.5f);
        LeanTween.moveLocalY(candidateA, startYPos, attackMoveTime);
        yield return new WaitForSeconds(0.2f);
        LeanTween.moveLocalY(candidateB, startYPos, attackMoveTime).setOnComplete(
             () =>
             {
                 StartCoroutine(VampireMove());
             }
         );
    }

    IEnumerator VampireMove()
    {
        yield return new WaitForSeconds(0.2f);
        float startYPos = vampireKnight.transform.position.y;
        float newYPos = vampireKnight.transform.position.y - attackMoveY;
        LeanTween.moveLocalY(vampireKnight, newYPos, attackMoveTime).setOnComplete(
            () =>
            {
                StartCoroutine(VampireAttack());
            }
            );
    }

    IEnumerator VampireAttack()
    {
        slashCandidateBAnimator1.SetTrigger("PlaySlash1");
        //yield return new WaitForSeconds(0.3f);
        slashCandidateBAnimator1.SetTrigger("PlaySlash4");
        yield return new WaitForSeconds(0.2f);
        slashCandidateBAnimator2.SetTrigger("PlaySlash2");

        yield return new WaitForSeconds(0.5f);
        float newXPos = vampireKnight.transform.position.x - 1;
        LeanTween.moveLocalX(vampireKnight, newXPos, 0.2f);
        yield return new WaitForSeconds(0.3f);
        slashCandidateAAnimator1.SetTrigger("PlaySlash5");
        //yield return new WaitForSeconds(0.3f);
        slashCandidateAAnimator1.SetTrigger("PlaySlash3");
        yield return new WaitForSeconds(0.3f);
        slashCandidateAAnimator2.SetTrigger("PlaySlash2");
        //yield return new WaitForSeconds(0.1f);
        slashCandidateAAnimator2.SetTrigger("PlaySlash1");

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CandidateGoBack());
    }

    IEnumerator CandidateGoBack()
    {
        float newYPos = candidateA.transform.position.y - retreatMoveY;
        LeanTween.moveLocalY(candidateA, newYPos, retreatMoveTime);
        yield return new WaitForSeconds(0.2f);
        LeanTween.moveLocalY(candidateB, newYPos, retreatMoveTime);
        yield return new WaitForSeconds(retreatMoveTime);
        StartCoroutine(PlayerComment());
    }

    IEnumerator PlayerComment()
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.actionAfterPause = CallCandidateGoBack2;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void CallCandidateGoBack2()
    {
        StartCoroutine(CandidateGoBack2());
    }

    IEnumerator CandidateGoBack2()
    {
        yield return new WaitForSeconds(0.3f);
        float newYPos = candidateA.transform.position.y - retreatMoveY;
        LeanTween.moveLocalY(candidateA, newYPos, retreatMoveTime).setOnComplete(
            () =>
            {
                candidateA.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("CandidateA_right", false);
            }
            );
        yield return new WaitForSeconds(0.1f);
        LeanTween.moveLocalY(candidateB, newYPos, retreatMoveTime).setOnComplete(
            () =>
            {
                candidateB.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("CandidateB_left", false);
            }
            );
        yield return new WaitForSeconds(retreatMoveTime + 0.5f);
        DialogueManager.instance.actionAfterPause = CallCandidatesTransportation;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void CallCandidatesTransportation()
    {
        StartCoroutine(CandidatesTransportation());
    }

    IEnumerator CandidatesTransportation()
    {
        yield return new WaitForSeconds(0.3f);
        candidateA.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("CandidateA_back", false);
        candidateB.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("CandidateB_back", false);
        yield return new WaitForSeconds(0.2f);
        transportationA.SetTrigger("PlayTransportation");
        AudioManager.instance.Play("transport", true, 0f);
        yield return new WaitForSeconds(0.2f);
        transportationB.SetTrigger("PlayTransportation");

        //vampire chases
        yield return new WaitForSeconds(1.1f);
        float newYPos = vampireKnight.transform.position.y - 2 + attackMoveY;
        LeanTween.moveLocalY(vampireKnight, newYPos, 0.4f);
        DialogueManager.instance.actionAfterPause = CallVampireTalk;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void CallVampireTalk()
    {
        StartCoroutine(VampireTalk());
    }

    IEnumerator VampireTalk()
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.actionAfterPause = CallVampireVomit;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void CallVampireVomit()
    {
        StartCoroutine(VampireVomit());
    }

    IEnumerator VampireVomit()
    {
        yield return new WaitForSeconds(1f);
        vomitBloodAnimate.SetTrigger("PlayBlood");
    }

    //trigger by animation function after vomit blood ended
    public IEnumerator VampireContinueToTalk()
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.actionAfterPause = CallVampireMoveBack;
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void CallVampireMoveBack()
    {
        StartCoroutine(VampireMoveBack());
    }

    IEnumerator VampireMoveBack()
    {
        yield return new WaitForSeconds(1f);
        vampireKnight.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_right", false);
        yield return new WaitForSeconds(0.5f);
        vampireKnight.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_back", false);
        yield return new WaitForSeconds(0.5f);
        float newYPos = vampireKnight.transform.position.y + 2;
        LeanTween.moveLocalY(vampireKnight, newYPos, 2f);
        yield return new WaitForSeconds(2f + 0.5f);

        vampireKnight.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_right", false);
        yield return new WaitForSeconds(0.5f);
        float newXPos = vampireKnight.transform.position.x + 1;
        LeanTween.moveLocalX(vampireKnight, newXPos, 1f);
        yield return new WaitForSeconds(1f + 0.5f);

        vampireKnight.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_back", false);
        yield return new WaitForSeconds(0.5f);
        newYPos = vampireKnight.transform.position.y + 2;
        LeanTween.moveLocalY(vampireKnight, newYPos, 2f);
        yield return new WaitForSeconds(2f + 1f);

        //player last talk
        DialogueManager.instance.ContinueDialogueAfterPause();
    }

    public void EndAnimation()
    {
        vampireKnightSecondStart.SetActive(true);
        vampireKnight.SetActive(false);
        GameManager.instance.AnimationEnded();
        Debug.Log("13th Floor Aniamtion Finish playing");
    }
    #endregion

    #region Fighting Part Animation
    //call after reaching plot empty object
    public IEnumerator StartSurpriseAttackAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        vampireKnightSecondStart.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_left", false);
        yield return new WaitForSeconds(0.2f);
        vampireKnightSecondStart.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_front", false);
        yield return new WaitForSeconds(0.2f);
        surpriseSlash.SetTrigger("PlaySlash2");
        yield return new WaitForSeconds(0.1f);
        surpriseSlash.SetTrigger("PlaySlash4");
        yield return new WaitForSeconds(0.5f);
        PlayerStatistics.instance.AddHP(-200, true);
        if (PlayerStatistics.instance.playerStats.GetHP() <= 0)
        {
            DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("13th_Floor_Lose"));
        }
        else
        {
            DialogueManager.instance.ContinueDialogueAfterPause();
        }
    }

    //start after surprise attack dialogue ended
    public IEnumerator StartDealAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        playerSlash.SetTrigger("PlaySlash5");
        yield return new WaitForSeconds(0.2f);
        surpriseSlash.SetTrigger("PlaySlash4");
        yield return new WaitForSeconds(0.5f);

        //caculate Attack Animation
        PlayerController.instance.UpdatePos(new Vector2(0, 1), false, false);
    }

    #endregion

    #region Fighting Win
    public IEnumerator FightingWin()
    {
        playerSlash.SetTrigger("PlaySlash3");
        playerSlash.SetTrigger("PlaySlash4");
        yield return new WaitForSeconds(0.8f);

        Destroy(vampireKnightSecondStart);
        UpdateUI.instance.ShowSwordIcon(true, UpdateUI.swordIcon2ShowedText);
        AnimationAllData.instance.is13FloorKilled = true;
        AnimationAllData.instance.is13FloorEscaped = false;
        GameObject.Find("Escape_Plot_Trigger").SetActive(false);
        DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("13th_Floor_Win"));
    }

    #endregion

    #region Fighting Lose
    public IEnumerator SurpriseLose()
    {
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("13th_Floor_Normal_Lose"));
    }
    public IEnumerator FightingLose()
    {
        yield return new WaitForSeconds(0.5f);
        surpriseSlash.SetTrigger("PlaySlash3");
        surpriseSlash.SetTrigger("PlaySlash4");

        Sound audioSource = AudioManager.instance.GetAudioSource("Level2_18_music");
        if (audioSource.source.isPlaying)
        {
            AudioManager.instance.SetAndFade(audioSource.name, 2f, audioSource.source.volume, 0f);
        }
        yield return new WaitForSeconds(1f);
        LeanTween.alpha(PlayerController.instance.gameObject, 0f, 1f);
        yield return new WaitForSeconds(1f);
        StartCoroutine(LoseUIManager.instance.ShowLoseUI());
    }
    #endregion

    #region Escape Fighting

    //trigger by entering exit without fighting vampire knight
    public IEnumerator EscapeAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("player_right", false);
        yield return new WaitForSeconds(0.5f);

        LeanTween.moveLocalX(PlayerController.instance.gameObject, playerEscapeEndPosX, 2f).setEase(LeanTweenType.easeOutSine).setOnComplete(
            () =>
            {
                StartCoroutine(AfterEscapeAnimation());
            });
    }

    private IEnumerator AfterEscapeAnimation()
    {
        LeanTween.alpha(PlayerController.instance.gameObject, 0f, 1f);
        yield return new WaitForSeconds(1f + 1f);
        DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("13th_Floor_Escape_Exit_Vampire"));
    }

    public IEnumerator EndEscapeAnimation()
    {
        yield return new WaitForSeconds(1f);
        float finalYPos = vampireKnightSecondStart.transform.position.y;
        finalYPos += 1f;
        LeanTween.moveLocalY(vampireKnightSecondStart, finalYPos, 2f);
        yield return new WaitForSeconds(2f + 0.5f);
        vampireKnightSecondStart.GetComponent<SpriteRenderer>().sprite = AddressableAssets.instance.GetSprite("vampire_knight_right", false);
        yield return new WaitForSeconds(1f);
        //manually set tile null
        DestroyVampireTile();

        AnimationAllData.instance.is13FloorKilled = false;
        AnimationAllData.instance.is13FloorEscaped = true;
        //activate longer transition only for this escape animation
        Image blackFade = GameObject.Find("Black_Canvas").GetComponentInChildren<Image>();
        LeanTween.alpha(blackFade.rectTransform, 1f, 1.5f).setOnComplete(
            () =>
            {
                GameManager.instance.SwitchFloor("downstairs");
            }
            );
    }

    private void DestroyVampireTile()
    {
        Vector3Int tilePos = new Vector3Int(-1, 3, 0);
        Tilemap tilemap = GameObject.Find("Enemy_Tilemap").GetComponent<Tilemap>();
        TilemapStatistics.instance.UpdateNullTiles(tilemap.gameObject.name, tilemap.GetTile(tilePos), tilePos, UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        tilemap.SetTile(tilePos, null);
    }

    #endregion
}
