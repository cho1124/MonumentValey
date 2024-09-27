using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Sequence/SequenceManager")]
public class SequenceManager : ScriptableObject
{
    public List<SequenceAction> actions;  // 실행할 동작 리스트
    

    // 시퀀스를 실행하는 메서드
    public void ExecuteSequence(Transform target, System.Action onComplete = null)
    {
        // DoTween 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 각각의 액션에 대해 처리
        foreach (SequenceAction action in actions)
        {
            Tween tween = action.CreateTween(target);  // 트윈 생성

            if (action.executionType == ActionExecutionType.Sequential)
            {
                // 순차 실행: Append로 트윈을 추가
                sequence.Append(tween);
            }
            else if (action.executionType == ActionExecutionType.Simultaneous)
            {
                // 동시 실행: Join으로 트윈을 추가
                sequence.Join(tween);
            }
        }

        if (onComplete != null)
        {
            sequence.OnComplete(() => onComplete());
        }

        // 시퀀스 시작
        sequence.Play();
    }
}
