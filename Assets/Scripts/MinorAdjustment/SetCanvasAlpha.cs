using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
//this class set fade black canvas alpha as 1 at start
public class SetCanvasAlpha : MonoBehaviour
{
    private Image blackFade;
    private void Start()
    {
        GameManager.instance.isEventPlaying = true;
        GameManager.instance.isAnimationPlaying = true;

        blackFade = gameObject.GetComponentInChildren<Image>();
        gameObject.GetComponentInChildren<Image>().color = new Color(0f, 0f, 0f, 1f);
        gameObject.GetComponent<CanvasGroup>().alpha = 1;

        if (FindObjectOfType<StartLoadManager>() == null)
        {
            LoadBlackFade();
        }
        else
        {
            if (!StartLoadManager.instance.isFileLoaded)
            {
                LoadBlackFade();
            }
        }
    }

    void LoadBlackFade()
    {
        //only fade if it is not loading file from start page

        if (SceneManager.GetActiveScene().buildIndex != 21)
        {
            SetAudio();

            //set visual
            LeanTween.alpha(blackFade.rectTransform, 0f, 0.5f).setOnComplete(
            () =>
            {
                //player controller store position also will activate isEventPlaying = false at start and scene change
                //so isAnimationPlaying is needed to add
                if (SceneManager.GetActiveScene().buildIndex == 14)
                {
                    if (AnimationAllData.instance.is13FloorAnimationEntryPlayed)
                    {
                        GameManager.instance.isEventPlaying = false;
                        GameManager.instance.isAnimationPlaying = false;
                    }
                }
                else
                {
                    GameManager.instance.isEventPlaying = false;
                    GameManager.instance.isAnimationPlaying = false;
                }
            }
            );
        }
        else
        {
            SetAudio();
        }
    }


    void SetAudio()
    {
        //set audio
        //fade in and stop specific background music
        AudioMixer audioMixer = Resources.Load<AudioMixer>("MasterMixer");
        audioMixer.SetFloat("MusicVol", -80f);
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer, "MusicVol", 0.5f, 1f));

        //fade in 13th or 19th music
        if (SceneManager.GetActiveScene().buildIndex == 20 || SceneManager.GetActiveScene().buildIndex == 14 || SceneManager.GetActiveScene().buildIndex == 21)
        {
            AudioManager.instance.Play("Level19_horror", false, 0f);
            if (AudioManager.instance.GetAudioSource("Level2_18_music").source.isPlaying)
            {
                AudioManager.instance.Stop("Level2_18_music");
            }
        }
        else
        {
            if (AudioManager.instance.GetAudioSource("Level19_horror").source.isPlaying)
            {
                AudioManager.instance.Stop("Level19_horror");
            }
        }

        //fade in level 2 - 18 music
        if (SceneManager.GetActiveScene().buildIndex >= 2 && SceneManager.GetActiveScene().buildIndex <= 19 && SceneManager.GetActiveScene().buildIndex != 14)
        {
            if (!AudioManager.instance.GetAudioSource("Level2_18_music").source.isPlaying)
            {
                //play once if it is not playing
                AudioManager.instance.Play("Level2_18_music", false, 0f);
            }
        }
        else
        {
            //don't play in 1st and 19th,20th floor
            if (AudioManager.instance.GetAudioSource("Level2_18_music").source.isPlaying)
            {
                AudioManager.instance.Stop("Level2_18_music");
            }
        }
    }
}
