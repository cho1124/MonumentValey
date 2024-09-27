using UnityEngine;
using DG.Tweening;

// �⺻ ������ ���� Ŭ���� (�߻� Ŭ����)

public enum ActionExecutionType
{
    Sequential,    // ���� ����
    Simultaneous   // ���� ����
}

public abstract class SequenceAction : ScriptableObject
{
    public ActionExecutionType executionType = ActionExecutionType.Sequential;
    // Ʈ�� ������ �����ϴ� �߻� �޼���
    public abstract Tween CreateTween(Transform target);
}
