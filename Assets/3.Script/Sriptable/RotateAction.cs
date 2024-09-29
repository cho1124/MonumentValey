using UnityEngine;
using DG.Tweening;  // DoTween 네임스페이스

[CreateAssetMenu(menuName = "SequenceActions/RotateAction")]
public class RotateAction : SequenceAction
{
    public Vector3 targetRotation;  // 이동할 목표 위치
    public float duration = 1f;     // 이동 시간
    public float delay = 0f;        // 딜레이 시간 추가


    // 동작 실행 (DoTween의 Sequence에 추가)
    public override Tween CreateTween(Transform target)
    {
        Tween tween = target.DOBlendableRotateBy(targetRotation, duration);
        return tween.SetDelay(delay);  // 딜레이 설정

    }
}
