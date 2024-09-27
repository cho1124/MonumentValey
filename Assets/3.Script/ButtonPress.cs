using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;
using UnityEngine.Events;

public enum ButtonType
{
    Once,
    Multiple
}

[System.Serializable]
public class SequenceTuple
{
    public SequenceManager sequenceManager;
    public Transform target;
}

public abstract class CommonPress : MonoBehaviour
{
    [Header("������ ��ü��")]
    [SerializeField] protected List<SequenceTuple> sequenceTuples;
    protected bool hasExecuted = false;
    public UnityEvent snappingEvent;

    protected abstract void ExecuteButtonAction();

}

public class ButtonPress : CommonPress
{
    [SerializeField] private ButtonType buttonType = ButtonType.Once;
    [SerializeField] private bool isPressed = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Node parentNode;
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        parentNode = GetComponentInParent<Node>();
        
    }

    private void Update()
    {
        Debug.Log(sequenceTuples[0].target.transform.rotation.eulerAngles);

        // parentNode.isStacked�� ���� ��ư ���� ����
        if (parentNode.isStacked)
        {
            isPressed = true;
        }
        else
        {
            // Multiple Ÿ���� ��ư�� �ٽ� ���� �� �ֵ��� ����
            if (buttonType == ButtonType.Multiple)
            {
                isPressed = false;
                hasExecuted = false;  // Multiple Ÿ�Կ����� �÷��� �ʱ�ȭ
            }
        }

        // �ִϸ����� ���� ������Ʈ
        UpdateAnimator(isPressed);

        // isPressed�� true�̰� ���� �������� ������� ���� ��� ������ ����
        if (isPressed && !hasExecuted)
        {
            ExecuteButtonAction();
            hasExecuted = true;  // ������ ���� �� �ٽ� ������� �ʵ��� �÷��� ����
        }
    }

    protected override void ExecuteButtonAction()
    {
        if (sequenceTuples.Count == 0) return;

        foreach(SequenceTuple s in sequenceTuples)
        {
            // ������ ����
            if (s.sequenceManager != null)
            {
                s.sequenceManager.ExecuteSequence(s.target, () => { Debug.Log("Action"); snappingEvent?.Invoke(); });  // ������ ����
            }
            
        }
    }


    private void UpdateAnimator(bool isPressed)
    {
        animator.SetBool("isPressed", isPressed);
    }

}
