using UnityEngine;
using DG.Tweening;  // DoTween ���ӽ����̽�

[CreateAssetMenu(menuName = "SequenceActions/MoveAction")]
public class MoveAction : SequenceAction
{
    public Vector3 targetPosition;  // �̵��� ��ǥ ��ġ
    public float duration = 1f;     // �̵� �ð�
   
    // ���� ���� (DoTween�� Sequence�� �߰�)
    public override Tween CreateTween(Transform target)
    {
        return target.DOMove(targetPosition, duration);
    }
}
