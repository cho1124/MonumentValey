using UnityEngine;
using DG.Tweening;

// 기본 시퀀스 동작 클래스 (추상 클래스)

public enum ActionExecutionType
{
    Sequential,    // 순차 실행
    Simultaneous   // 동시 실행
}

public abstract class SequenceAction : ScriptableObject
{
    public ActionExecutionType executionType = ActionExecutionType.Sequential;
    // 트윈 동작을 실행하는 추상 메서드
    public abstract Tween CreateTween(Transform target);
}
