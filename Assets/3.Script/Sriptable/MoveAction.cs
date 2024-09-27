using UnityEngine;
using DG.Tweening;  // DoTween 네임스페이스

[CreateAssetMenu(menuName = "SequenceActions/MoveAction")]
public class MoveAction : SequenceAction
{
    public Vector3 targetPosition;  // 이동할 목표 위치
    public float duration = 1f;     // 이동 시간
   
    // 동작 실행 (DoTween의 Sequence에 추가)
    public override Tween CreateTween(Transform target)
    {
        return target.DOMove(targetPosition, duration);
    }
}
