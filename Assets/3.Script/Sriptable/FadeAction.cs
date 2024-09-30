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
            Debug.LogError("Image�� �ʿ��մϴ�.");
            return null;
        }

        // ���̵� ��/�ƿ� Ʈ�� ����
        if (fadeIn)
        {
            return image.DOFade(1f, duration); // ���̵� �� (���� 1��)
        }
        else
        {
            return image.DOFade(0f, duration); // ���̵� �ƿ� (���� 0����)
        }
    }



}
