using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ScreenShotHandler : MonoBehaviour
{
    //private string screenShotPath;

    public Texture2D screenShotTexture;

    public static ScreenShotHandler instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void StartScreenCapture()
    {
        StartCoroutine(CaptureScreen());
    }

    IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log("Capture ScreeShot");
        screenShotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        UpdateUI.instance.OpenMainMenu(UpdateUI.instance.defaultSelectedButton);
    }

    //convert texture2d to sprite and return to UpdateUI
    public Sprite GetSavedScreenShot()
    {
        Sprite screenShotSprite = Sprite.Create(
            screenShotTexture,
            new Rect(0.0f, 0.0f, screenShotTexture.width, screenShotTexture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
            );
        return screenShotSprite;
    }
}
