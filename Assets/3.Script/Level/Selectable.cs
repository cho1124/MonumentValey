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

    private IState currentState; // ���� ���� ����
    private ClosedState closedState; // Idle ���� ��ü
    private OpeningState openingState; // Opening ���� ��ü
    private OpenedState openedState; // Opened ���� ��ü

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

        
        // �� �κ� json���� �ҷ������� � ����̵��� �Ἥ ���¸� ���������� ��ȯ��Ű���� �� ��
        ChangeState(closedState);
    }

    public void ChangeState(IState newState)
    {
        // ���� ���¿��� ������
        currentState?.Exit();

        // ���ο� ���·� ����
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

// Idle ���� Ŭ����
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
        // Idle ���� ���� �� ó���� ����
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
        //Debug.Log("Opening ���¿� ����");
        ani.Play("Unlocking");
    }

    public void Execute()
    {

        // �ִϸ��̼� ���� ���� �� ����
        if (!ani.isPlaying)
        {
            // �ִϸ��̼��� ������ Opened ���·� ��ȯ
            selectable.ChangeState(new OpenedState(selectable, ani));
        }
        
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
    private Animation ani;

    public OpenedState(Selectable selectable, Animation ani)
    {
        this.selectable = selectable;
        this.ani = ani;
    }

    public void Enter()
    {
        Debug.Log("Opened ���¿� ����");
        ani.Play("Open_4");
    }

    public void Execute()
    {
        // Opened ���� ���� �� ó���� ����
    }

    public void Exit()
    {
        Debug.Log("Opened ���¿��� ����");
    }
}

#endregion