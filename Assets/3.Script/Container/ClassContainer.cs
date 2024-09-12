using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum StageState
{
    Closed,
    Opening,
    Opened
}


public class Sequence
{
    private Queue<UnityAction> actions = new Queue<UnityAction>();
    private MonoBehaviour owner;

    public Sequence(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public Sequence AddAction(UnityAction action)
    {
        actions.Enqueue(action);

        return this;
    }

    public void StartSequence()
    {
        ExecuteNextAction();
    }

    private void ExecuteNextAction()
    {
        if (actions.Count > 0)
        {
            UnityAction action = actions.Dequeue();
            action.Invoke(); // 액션 실행
            ExecuteNextAction(); // 다음 액션 실행
        }
        else
        {
            Debug.Log("모든 액션이 완료되었습니다.");
        }
    }
}




[Serializable]
public class Stage
{
    public int stageNum;
    public string stageName;
    public GameObject stageObj;
    
}

