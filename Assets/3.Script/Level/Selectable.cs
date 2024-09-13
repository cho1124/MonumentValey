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

    [SerializeField] private IState currentState; // ���� ���� ����
    private ClosedState closedState; // Idle ���� ��ü
    private OpeningState openingState; // Opening ���� ��ü
    private OpenedState openedState; // Opened ���� ��ü
    private AnimatorStateInfo currentAniStateInfo;

    public IState GetClosedState() => closedState;
    public IState GetOpeningState() => openingState;
    public IState GetOpenedState() => openedState;
    public IState GetCurrentState() => currentState;

    private void Awake()
    {
        ani = GetComponentInChildren<Animator>();

        //�� �κ� ���� �ʿ�
        closedState = new ClosedState(this, ani, "Closed");
        openingState = new OpeningState(this, ani, "Unlocking");
        openedState = new OpenedState(this, ani, "Open_4");

        // �� �κ� json���� �ҷ������� � ����̵��� �Ἥ ���¸� ���������� ��ȯ��Ű���� �� ��
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
        //�� �κ� �����ؾ��� �� Ư���� �̸��� �ƴϸ� ���ѷ��� ���� �׻� ���� �� ����, string�� ���� �κ��� ĳ���ϴ°͵� ������ ���� ��
        while (true)
        {
            currentAniStateInfo = ani.GetCurrentAnimatorStateInfo(0);

            if (currentAniStateInfo.IsName("Unlocking") && currentAniStateInfo.normalizedTime >= 0.9f)
            {
                break;
            }

            yield return null; // ���� �����ӱ��� ���
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

// Idle ���� Ŭ����
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
        Debug.Log("Opening ���¿��� ����");
    }
}

// Opened ���� Ŭ����
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
        //Debug.Log("Opened ���¿� ����");
        //Debug.Log("animation name : " + animationName);
        ani.Play(animationName);
    }



    public void Exit()
    {
        //Debug.Log("Opened ���¿��� ����");
    }
}

#endregion