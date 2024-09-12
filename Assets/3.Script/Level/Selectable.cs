using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{

    // Start is called before the first frame update
    public Stage stage;

    public Animation ani;
    public string animationClipName;

    private IState currentState; // 현재 상태 저장
    private ClosedState closedState; // Idle 상태 객체
    private OpeningState openingState; // Opening 상태 객체
    private OpenedState openedState; // Opened 상태 객체

    public IState GetClosedState() => closedState;
    public IState GetOpeningState() => openingState;
    public IState GetOpenedState() => openedState;
    public IState GetCurrentState() => currentState;


    private void Start()
    {
        ani = GetComponentInChildren<Animation>();

        
        closedState = new ClosedState(this, ani);
        openingState = new OpeningState(this, ani);
        openedState = new OpenedState(this, ani);

        
        // 이 부분 json으로 불러오던지 어떤 방법이든지 써서 상태를 유동적으로 변환시키도록 할 것
        ChangeState(closedState);
    }

    public void ChangeState(IState newState)
    {
        // 현재 상태에서 나가기
        currentState?.Exit();

        // 새로운 상태로 진입
        currentState = newState;
        currentState.Enter();
    }

    

   
    


}
#region StateMachine
public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

// Idle 상태 클래스
public class ClosedState : IState
{
    private Selectable selectable;
    private Animation ani;

    public ClosedState(Selectable selectable, Animation ani)
    {
        this.selectable = selectable;
        this.ani = ani;
    }

    public void Enter()
    {
        
    }

    public void Execute()
    {
        // Idle 상태 유지 중 처리할 로직
    }

    public void Exit()
    {
        
    }
}

public class OpeningState : IState
{
    private Selectable selectable;
    private Animation ani;

    public OpeningState(Selectable selectable, Animation ani)
    {
        this.selectable = selectable;
        this.ani = ani;
    }

    public void Enter()
    {
        //Debug.Log("Opening 상태에 진입");
        ani.Play("Unlocking");
    }

    public void Execute()
    {

        // 애니메이션 진행 중일 때 로직
        if (!ani.isPlaying)
        {
            // 애니메이션이 끝나면 Opened 상태로 전환
            selectable.ChangeState(new OpenedState(selectable, ani));
        }
        
    }

    public void Exit()
    {
        Debug.Log("Opening 상태에서 나감");
    }
}

// Opened 상태 클래스
public class OpenedState : IState
{
    private Selectable selectable;
    private Animation ani;

    public OpenedState(Selectable selectable, Animation ani)
    {
        this.selectable = selectable;
        this.ani = ani;
    }

    public void Enter()
    {
        Debug.Log("Opened 상태에 진입");
        ani.Play("Open_4");
    }

    public void Execute()
    {
        // Opened 상태 유지 중 처리할 로직
    }

    public void Exit()
    {
        Debug.Log("Opened 상태에서 나감");
    }
}

#endregion