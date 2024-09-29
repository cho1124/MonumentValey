using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[CreateAssetMenu(menuName = "SequenceActions/RotateFixedAction")]
public class RotateFixedAction : SequenceAction
{
    public Vector3 targetRotation;  // �̵��� ��ǥ ��ġ
    public float duration = 1f;     // �̵� �ð�
    public float delay = 0f;    


    // ���� ���� (DoTween�� Sequence�� �߰�)
    public override Tween CreateTween(Transform target)
    {
        //return target.DOBlendableRotateBy(targetRotation, duration);
        Tween tween = target.DORotate(targetRotation, duration);
        return tween.SetDelay(delay);

    }
}
