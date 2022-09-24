using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation12thFloorManager : MonoBehaviour
{
    public static Animation12thFloorManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public IEnumerator Animation12thFloorDownstairs()
    {
        GameManager.instance.isAnimationPlaying = true;
        yield return new WaitForSeconds(0.5f);
        Vector3 playerLocalPos = PlayerController.instance.gameObject.transform.localPosition;
        playerLocalPos.y = playerLocalPos.y - 1f;
        LeanTween.moveLocalY(PlayerController.instance.gameObject, playerLocalPos.y, 2f).setEase(LeanTweenType.easeOutSine).setOnComplete(
            () => {
                GameManager.instance.SwitchFloor("downstairs");
            }
            );
    }
}
