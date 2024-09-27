using UnityEngine;
using DG.Tweening;  // DoTween ���ӽ����̽�

[CreateAssetMenu(menuName = "SequenceActions/RotateAction")]
public class RotateAction : SequenceAction
{
    public Vector3 targetRotation;  // �̵��� ��ǥ ��ġ
    public float duration = 1f;     // �̵� �ð�
    

    // ���� ���� (DoTween�� Sequence�� �߰�)
    public override Tween CreateTween(Transform target)
    {
        return target.DOBlendableRotateBy(targetRotation, duration);
        //return target.DORotate(targetRotation, duration);
        
    }
}
