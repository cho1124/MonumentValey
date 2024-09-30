using UnityEngine;
using DG.Tweening; 

[CreateAssetMenu(menuName = "SequenceActions/MoveAction")]
public class MoveAction : SequenceAction
{
    public Vector3 targetPosition;  // �̵��� ��ǥ ��ġ
    public float duration = 1f;     // �̵� �ð�
    public float delay = 0f;
   
    // ���� ���� (DoTween�� Sequence�� �߰�)
    public override Tween CreateTween(Transform target)
    {
        Tween tween = target.DOMove(targetPosition, duration);

        return tween.SetDelay(delay);
    }
}
