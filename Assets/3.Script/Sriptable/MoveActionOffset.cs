using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "SequenceActions/MoveActionOffset")]
public class MoveActionOffset : SequenceAction
{
    public Vector3 offsetPosition;  // 이동할 목표 위치
    public float duration = 1f;     // 이동 시간
    public float delay = 0f;

    // 동작 실행 (DoTween의 Sequence에 추가)
    public override Tween CreateTween(Transform target)
    {
        Tween tween = target.DOMove(target.position + offsetPosition, duration);

        return tween.SetDelay(delay);
    }
}
