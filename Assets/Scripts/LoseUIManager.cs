using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class LoseUIManager : MonoBehaviour
{

    //lose UI
    private GameObject loseUI;
    private Image loseBackground;
    private TextMeshProUGUI loseText;
    private Button loseButton;

    public static LoseUIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        loseUI = GameObject.Find("Lose_UI");
        loseUI.SetActive(false);
        loseUI.GetComponent<CanvasGroup>().alpha = 1;

        loseBackground = loseUI.transform.Find("Lose_Background").GetComponent<Image>();
        loseText = loseUI.transform.Find("Lose_Text").GetComponent<TextMeshProUGUI>();
        loseText.alpha = 0;
        loseButton = loseUI.transform.Find("Lose_Button").GetComponent<Button>();
        loseButton.GetComponentInChildren<TextMeshProUGUI>().alpha = 0;
    }

    public IEnumerator ShowLoseUI()
    {
        //audio
        AudioManager.instance.ResetPlay("lose_muisc");

        //visual
        loseUI.SetActive(true);
        LeanTween.alpha(loseBackground.rectTransform, 1f, 2f);

        //move lose text
        yield return new WaitForSeconds(1f);
        Vector3 loseTextPos = loseText.gameObject.transform.localPosition;
        float loseTextEndYPos = loseTextPos.y;
        float loseTextStartPos = loseTextEndYPos - 20f;
        loseText.gameObject.transform.localPosition = new Vector3(loseTextPos.x, loseTextStartPos, loseTextPos.z);
        LeanTweenExt.LeanAlphaText(loseText, 1f, 1f);
        LeanTween.moveLocalY(loseText.gameObject, loseTextEndYPos, 2f).setEase(LeanTweenType.easeOutSine);

        //move lose button
        yield return new WaitForSeconds(0.1f);
        Vector3 loseButtonPos = loseButton.gameObject.transform.localPosition;
        float loseButtonEndYPos = loseButtonPos.y;
        float loseButtonStartPos = loseButtonEndYPos - 20f;
        loseButton.gameObject.transform.localPosition = new Vector3(loseButtonPos.x, loseButtonStartPos, loseButtonPos.z);

        LeanTweenExt.LeanAlphaText(loseButton.GetComponentInChildren<TextMeshProUGUI>(), 1f, 1f);
        LeanTween.moveLocalY(loseButton.gameObject, loseButtonEndYPos, 2f).setEase(LeanTweenType.easeOutSine).setOnComplete(
            () => {
                loseButton.onClick.AddListener(
                    () =>
                    {
                        InputSystem.DisableDevice(Keyboard.current);
                        AudioManager.instance.Play("button_exit", true, 0f);
                        GameManager.instance.BackToTitle();
                    }
                    );
                loseButton.Select();
            });
    }
}
