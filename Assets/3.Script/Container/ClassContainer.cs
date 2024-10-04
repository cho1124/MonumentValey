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

[Serializable]
public class Stage
{
    public int stageNum;
    public string stageName;
    public GameObject stageObj;
    public IState StartState;

}

