using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Sequence/SequenceManager")]
public class SequenceManager : ScriptableObject
{
    public List<SequenceAction> actions;  // ������ ���� ����Ʈ
    

    // �������� �����ϴ� �޼���
    public void ExecuteSequence(Transform target, System.Action onComplete = null)
    {
        // DoTween ������ ����
        Sequence sequence = DOTween.Sequence();

        // ������ �׼ǿ� ���� ó��
        foreach (SequenceAction action in actions)
        {
            Tween tween = action.CreateTween(target);  // Ʈ�� ����

            if (action.executionType == ActionExecutionType.Sequential)
            {
                // ���� ����: Append�� Ʈ���� �߰�
                sequence.Append(tween);
            }
            else if (action.executionType == ActionExecutionType.Simultaneous)
            {
                // ���� ����: Join���� Ʈ���� �߰�
                sequence.Join(tween);
            }
        }

        if (onComplete != null)
        {
            sequence.OnComplete(() => onComplete());
        }

        // ������ ����
        sequence.Play();
    }
}
