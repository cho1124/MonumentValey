using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    // Start is called before the first frame update
    public Stage stage;
    public Animator ani;
    public string animationClipName;

    [SerializeField] private IState currentState; // 현재 상태 저장
    private ClosedState closedState; // Idle 상태 객체
    private OpeningState openingState; // Opening 상태 객체
    private OpenedState openedState; // Opened 상태 객체
    private AnimatorStateInfo currentAniStateInfo;

    public IState GetClosedState() => closedState;
    public IState GetOpeningState() => openingState;
    public IState GetOpenedState() => openedState;
    public IState GetCurrentState() => currentState;

    private void Awake()
    {
        ani = GetComponentInChildren<Animator>();

        //이 부분 수정 필요
        closedState = new ClosedState(this, ani, "Closed");
        openingState = new OpeningState(this, ani, "Unlocking");
        openedState = new OpenedState(this, ani, "Open_4");

        // 이 부분 json으로 불러오던지 어떤 방법이든지 써서 상태를 유동적으로 변환시키도록 할 것
        currentState = closedState;

    }
    private void OnEnable()
    {
        ChangeState(currentState);
    }

    private void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        if (currentState is OpeningState)
        {
            currentState.Enter();
            
            StartCoroutine(OpeningSequence());
        }
        else
        {
            currentState.Enter();
        }

    }
    public void Closed()
    {
        ChangeState(closedState);
    }

    public void Opening()
    {
        ChangeState(openingState);
    }

    public void Open()
    {
        ChangeState(openedState);
    }

    private IEnumerator OpeningSequence()
    {
        //이 부분 조심해야할 것 특정한 이름이 아니면 무한루프 도니 항상 조심 또 조심, string에 대한 부분은 캐싱하는것도 나쁘지 않을 듯
        while (true)
        {
            currentAniStateInfo = ani.GetCurrentAnimatorStateInfo(0);

            if (currentAniStateInfo.IsName("Unlocking") && currentAniStateInfo.normalizedTime >= 0.9f)
            {
                break;
            }

            yield return null; // 다음 프레임까지 대기
        }

        Open();

    }
    public bool IsOpeningOrOpened()
    {
        return currentState is OpeningState || currentState is OpenedState;
    }

    public bool IsClosed()
    {
        return currentState is ClosedState;
    }


}
#region StateMachine

public interface IState
{
    void Enter();
    void Exit();
}

// Idle 상태 클래스
public class ClosedState : IState
{
    private Selectable selectable;
    private Animator ani;
    private string animationName;

    public ClosedState(Selectable selectable, Animator ani, string animationName)
    {
        this.selectable = selectable;
        this.ani = ani;
        this.animationName = animationName;

    }

    public void Enter()
    {
        ani.Play(animationName);
    }

    public void Exit()
    {

    }
}

public class OpeningState : IState
{
    private Selectable selectable;
    private Animator ani;
    private string animationName;

    public OpeningState(Selectable selectable, Animator ani, string animationName)
    {
        this.selectable = selectable;
        this.ani = ani;
        this.animationName = animationName;

    }

    public void Enter()
    {
        Debug.Log("animationName : " + animationName);
        ani.Play(animationName);

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
    private Animator ani;
    private string animationName;

    public OpenedState(Selectable selectable, Animator ani, string animationName)
    {
        this.selectable = selectable;
        this.ani = ani;
        this.animationName = animationName;
    }

    public void Enter()
    {
        //Debug.Log("Opened 상태에 진입");
        //Debug.Log("animation name : " + animationName);
        ani.Play(animationName);
    }



    public void Exit()
    {
        //Debug.Log("Opened 상태에서 나감");
    }
}

#endregion