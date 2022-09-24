using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System;

//This class controls the player movemenet and hit signals
public class PlayerController : MonoBehaviour
{
    //player sprites
    [SerializeField]
    private Sprite playerSpriteFront;
    [SerializeField]
    private Sprite playerSpriteRight;
    [SerializeField]
    private Sprite playerSpriteBack;
    [SerializeField]
    private Sprite playerSpriteLeft;

    //interval of movement
    private float moveTimeInterval = 0.2f;
    private bool movable = true;

    public Vector3 updatePos;
    //check for any object occupying the block which is Player's destination
    private float rayCastDistance = 1f;

    //only allow one key pressed at a time
    private int keyCounter = 0;

    //get addressable sprite
    private SpriteRenderer playerSpriteCurrent;

    public static PlayerController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        updatePos = gameObject.GetComponent<Transform>().localPosition;
        playerSpriteCurrent = gameObject.GetComponent<SpriteRenderer>();
        movable = false;
        UpdatePlayerPosBewteenScene();
        StartCoroutine(Move());
    }

    void Update()
    {
        //two parameter is needed to prevent two instant keys are pressed and moves
        if (GameManager.instance.isEventPlaying || !movable)
        {
            return;
        }
        //check 4 directions of movement
        //When Player goes down or goes left, hitPosY or hitPosX maybe 2.0 but the tile posY or X should be 1.0
        GoDown();
        GoUp();
        GoRight();
        GoLeft();
    }

    private void GoDown()
    {
        if (Keyboard.current.downArrowKey.isPressed & keyCounter < 1 || Keyboard.current.sKey.isPressed & keyCounter < 1)
        {
            UpdatePos(new Vector2(0, -1), false, true);
            playerSpriteCurrent.sprite = playerSpriteFront;
        }
    }
    private void GoUp()
    {
        if (Keyboard.current.upArrowKey.isPressed & keyCounter < 1 || Keyboard.current.wKey.isPressed & keyCounter < 1)
        {
            UpdatePos(new Vector2(0, 1), false, false);
            playerSpriteCurrent.sprite = playerSpriteBack;
        }
    }
    private void GoRight()
    {
        if (Keyboard.current.rightArrowKey.isPressed & keyCounter < 1 || Keyboard.current.dKey.isPressed & keyCounter < 1)
        {
            UpdatePos(new Vector2(1, 0), false, false);
            playerSpriteCurrent.sprite = playerSpriteRight;
        }
    }
    private void GoLeft()
    {
        if (Keyboard.current.leftArrowKey.isPressed & keyCounter < 1 || Keyboard.current.aKey.isPressed & keyCounter < 1)
        {
            UpdatePos(new Vector2(-1, 0), true, false);
            playerSpriteCurrent.sprite = playerSpriteLeft;
        }
    }

    //update Player position and detect collision
    public void UpdatePos(Vector2 moveDirection, bool leftPressed, bool downPressed)
    {
        keyCounter += 1;
        movable = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(moveDirection), rayCastDistance);
        //collision triggered
        GameManager.instance.isEventPlaying = true;
        if (hit)
        {
            if (hit.collider.tag != "Player")
            {
                //these tags of object does not contain tilemap
                if (hit.collider.tag == "Plot_Empty")
                {
                    DialogueTrigger dialogueTrigger = null;
                    dialogueTrigger = hit.collider.gameObject.GetComponent<DialogueTrigger>();
                    StartCoroutine(dialogueTrigger.OnRayCastEnterEmptyPlot(hit));
                    MovePlayer(moveDirection, true);
                }
                //other tag gameobject contain tilemap data
                else if (hit.collider.tag != "Wall")
                {
                    string tagName = hit.collider.tag;
                    LayerCheck.instance.CheckTag(tagName, hit, leftPressed, downPressed);
                }
                else if (hit.collider.tag == "Wall")
                {
                    GameManager.instance.isEventPlaying = false;
                }
                else
                {
                    GameManager.instance.isEventPlaying = false;
                    Debug.LogWarning("No Hit tag matched");
                }
            }
        }
        //move when no collision triggered
        else
        {
            MovePlayer(moveDirection);
        }
        //allow to move again after a few moment
        StartCoroutine("Move");
    }

    //allow Player to move again after a period of time
    IEnumerator Move()
    {
        yield return new WaitForSeconds(moveTimeInterval);
        movable = true;
        keyCounter = 0;
    }
    
    void MovePlayer(Vector2 moveDirection, bool isDialoguePlaying = false)
    {
        updatePos = new Vector3(updatePos.x + moveDirection.x, updatePos.y + moveDirection.y, updatePos.z);
        transform.localPosition = updatePos;
        StorePlayerPos(isDialoguePlaying);
    }

    //update player position when enter next or previous level, or using fly tool
    public void UpdatePlayerPosBewteenScene()
    {
        Vector2 vector2 = GameManager.instance.ReturnPlayerStartPos();
        gameObject.transform.localPosition = new Vector3 (vector2.x, vector2.y, transform.localPosition.z);
        updatePos = gameObject.transform.localPosition;
        StorePlayerPos();
    }

    //show player move to stair, isEventPlaying is still true, wait for animation to transit to next floor
    public void MoveToStairs(Vector2 moveDirection)
    {
        updatePos = new Vector3(updatePos.x + moveDirection.x, updatePos.y + moveDirection.y, updatePos.z);
        transform.localPosition = updatePos;
    }

    //move and store position when
    //normal move
    //move which trigger event
    //end of animation move
    public void StorePlayerPos(bool isDialoguePlaying = false)
    {
        PlayerStatistics.instance.playerStats.SetPosition(transform.localPosition);
        if (!isDialoguePlaying && !GameManager.instance.isAnimationPlaying)
        {
            //let player to make action again if it is not a move that trigger dialogue/plot_empty
            GameManager.instance.isEventPlaying = false;
        }
    }

    public void LoadPlayerPos(Vector3 playerPosition)
    {
        updatePos = playerPosition;
        this.gameObject.transform.position = updatePos;
    }
}