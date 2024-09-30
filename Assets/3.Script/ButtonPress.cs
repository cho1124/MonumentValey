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
    [SerializeField] private bool isSim = false;
    [SerializeField] private float delayTime = 0.5f;
    [SerializeField] private Animator targetAnimator;


    private void Start()
    {
        animator = GetComponent<Animator>();
        parentNode = GetComponentInParent<Node>();
        
    }

    private void Update()
    {
        //Debug.Log(sequenceTuples[0].target.transform.position);

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

        if(targetAnimator != null)
        {
            targetAnimator.SetBool("isOpen", true);
        }


        if(isSim)
        {
            StartCoroutine(ExecuteSequencesSimulately());
        }
        else
        {
            StartCoroutine(ExecuteSequencesSequentially());
        }

    }

    // �������� ���������� �����ϴ� �ڷ�ƾ
    private IEnumerator ExecuteSequencesSequentially()
    {
        foreach (SequenceTuple s in sequenceTuples)
        {
            bool sequenceCompleted = false;

            // �������� ���� �ڿ� sequenceCompleted�� true�� ����
            if (s.sequenceManager != null)
            {
                s.sequenceManager.ExecuteSequence(s.target, () =>
                {
                    Debug.Log("Action Completed");
                    snappingEvent?.Invoke();  // �̺�Ʈ ȣ��
                    sequenceCompleted = true;  // ������ �Ϸ�
                });
            }

            // �������� �Ϸ�� ������ ���
            yield return new WaitUntil(() => sequenceCompleted);
        }

        Debug.Log("All sequences completed.");
    }

    private IEnumerator ExecuteSequencesSimulately()
    {
        foreach (SequenceTuple s in sequenceTuples)
        {
            

            // �������� ���� �ڿ� sequenceCompleted�� true�� ����
            if (s.sequenceManager != null)
            {
                s.sequenceManager.ExecuteSequence(s.target, () =>
                {
                    Debug.Log("Action Completed");
                    snappingEvent?.Invoke();  // �̺�Ʈ ȣ��
                    
                });
            }

            // �������� �Ϸ�� ������ ���
            yield return new WaitForSeconds(delayTime);
            
        }
        

        Debug.Log("All sequences completed.");
    }



    private void UpdateAnimator(bool isPressed)
    {
        animator.SetBool("isPressed", isPressed);
    }

}
