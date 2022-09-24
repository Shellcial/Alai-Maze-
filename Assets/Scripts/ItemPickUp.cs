using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class handles events of items that will be picked up immediately and have instant effects
//wihch includes potions, gems and weapon
public class ItemPickUp : MonoBehaviour
{
    private int redHP = 50;
    private int blueHP = 200;

    private int redATK = 1;
    private int blueDEF = 1;

    private int swordATK = 10;
    private int shieldDEF = 15;
    public void PickUP(string tileName)
    {
        AudioManager.instance.Play("get_item", true, 0f);

        switch (tileName)
        {
            case "red_potion":
                PlayerStatistics.instance.AddHP(redHP);
                break;
            case "blue_potion":
                PlayerStatistics.instance.AddHP(blueHP);
                break;
            case "red_gem":
                PlayerStatistics.instance.AddATK(redATK);
                break;
            case "blue_gem":
                PlayerStatistics.instance.AddDEF(blueDEF);
                break;
            case "sword":
                PlayerStatistics.instance.AddATK(swordATK);
                UpdateUI.instance.ShowSwordIcon(true, UpdateUI.swordIconShowedText);
                break;
            case "shield":
                PlayerStatistics.instance.AddDEF(shieldDEF);
                UpdateUI.instance.ShowShieldIcon(true);
                break;
            case "alai_gem":
                UpdateUI.instance.ChangeUpgradeMenu(false);
                UpdateUI.instance.ShowGemIcon(true, UpdateUI.gemIconShowedText);
                //trigger dialogue
                DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("3rd_Floor_MP_Usage"));
                AddressableAssets.instance.ReleaseDialogueHandle();
                break;
            case "alai_gem_purple":
                UpdateUI.instance.ChangeUpgradeMenu(true);
                UpdateUI.instance.ShowGemIcon(true, UpdateUI.gemIcon2ShowedText);
                UpdateUI.instance.ShowSwordIcon(true, UpdateUI.swordIcon3ShowedText);
                //trigger dialogue
                DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("14th_Floor_Gem"));
                AddressableAssets.instance.ReleaseDialogueHandle();
                break;
            case "alai_gem_purple_pure":
                UpdateUI.instance.ChangeUpgradeMenu(true);
                UpdateUI.instance.ShowGemIcon(true, UpdateUI.gemIcon3ShowedText);
                //trigger dialogue
                DialogueManager.instance.StartDialogue(AddressableAssets.instance.GetDialogue("14th_Floor_Pure_Gem"));
                AddressableAssets.instance.ReleaseDialogueHandle();
                break;
            default:
                Debug.LogWarning("no tile name is matched in pickup");
                break;
        }
    }
}
