using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "SequenceActions/FadeAction")]
public class FadeAction : SequenceAction
{
    public float duration = 1f;
    public bool fadeIn = true;

    public override Tween CreateTween(Transform target)
    {
        Image image = target.GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError("Image가 필요합니다.");
            return null;
        }

        // 페이드 인/아웃 트윈 생성
        if (fadeIn)
        {
            return image.DOFade(1f, duration); // 페이드 인 (알파 1로)
        }
        else
        {
            return image.DOFade(0f, duration); // 페이드 아웃 (알파 0으로)
        }
    }



}
