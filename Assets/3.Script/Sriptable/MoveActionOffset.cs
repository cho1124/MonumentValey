using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "SequenceActions/MoveActionOffset")]
public class MoveActionOffset : SequenceAction
{
    public Vector3 offsetPosition;  // �̵��� ��ǥ ��ġ
    public float duration = 1f;     // �̵� �ð�
    public float delay = 0f;

    // ���� ���� (DoTween�� Sequence�� �߰�)
    public override Tween CreateTween(Transform target)
    {
        Tween tween = target.DOMove(target.position + offsetPosition, duration);

        return tween.SetDelay(delay);
    }
}
