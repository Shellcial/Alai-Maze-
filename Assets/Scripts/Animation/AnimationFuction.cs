using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//this class allows object with animator can invoke function in animation clip
public class AnimationFuction : MonoBehaviour
{
    private int teleportIndex;


    void Start()
    {
        teleportIndex = 0;
    }

    //for transportation
    public void HideObject()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (sceneIndex)
        {
            case 14:
                Animation13thFloorManager.instance.candidateA.GetComponent<SpriteRenderer>().enabled = false;
                Animation13thFloorManager.instance.candidateB.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 20:
                //transport Player
                PlayerController.instance.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 21:
                if (teleportIndex == 0 )
                {
                    Animation20thFloorManager.instance.candidateB.GetComponent<SpriteRenderer>().enabled = false; ;
                }
                else if (teleportIndex == 1)
                {
                    Animation20thFloorManager.instance.vampireKnight.GetComponent<SpriteRenderer>().enabled = false; ;
                    Animation20thFloorManager.instance.vampirePrincess.GetComponent<SpriteRenderer>().enabled = false; ;
                }
                teleportIndex += 1;
                //transport Vampire Knight and Vampire Princess
                break;
        }
    }
    //after slash animation completed
    public void ResetSlashTrigger()
    {
        gameObject.GetComponent<Animator>().SetTrigger("ResetSlash");
    }

    //after transportation animation completed
    public void ResetTransportTrigger()
    {
        gameObject.GetComponent<Animator>().SetTrigger("ResetTransportation");
    }

    public void PlaySlashSound(string index)
    {
        switch (index)
        {
            case "1":
                AudioManager.instance.Play("slash1", true, 0.027f);
                break;
            case "2":
                AudioManager.instance.Play("slash2", true, 0.031f);
                break;
            case "3":
                AudioManager.instance.Play("slash3", true, 0.042f);
                break;
            case "4":
                AudioManager.instance.Play("slash4", true, 0f);
                break;
            case "5":
                AudioManager.instance.Play("slash5", true, 0.013f);
                break;
            case "blocked":
                AudioManager.instance.Play("slash1", true, 0.06f);
                break;
        }
            
    }

    //call after vomit blood on 13th floor
    public void BloodFinsihed()
    {
        StartCoroutine(Animation13thFloorManager.instance.VampireContinueToTalk());
    }

}
