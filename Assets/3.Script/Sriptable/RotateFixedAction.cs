using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[CreateAssetMenu(menuName = "SequenceActions/RotateFixedAction")]
public class RotateFixedAction : SequenceAction
{
    public Vector3 targetRotation;  // 이동할 목표 위치
    public float duration = 1f;     // 이동 시간
    public float delay = 0f;    


    // 동작 실행 (DoTween의 Sequence에 추가)
    public override Tween CreateTween(Transform target)
    {
        //return target.DOBlendableRotateBy(targetRotation, duration);
        Tween tween = target.DORotate(targetRotation, duration);
        return tween.SetDelay(delay);

    }
}
