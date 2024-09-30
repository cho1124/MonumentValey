using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;

public class DoorPress : CommonPress
{
    public bool isOpened = false;
    private Animator animator;
    private Node thisNode;
    [SerializeField] private Node nextStartNode;
    [SerializeField] private Node nextMovetoNode;
    [SerializeField] private bool isSim = false;
    [SerializeField] private float delayTime = 0.5f;


    private void Start()
    {
        thisNode = GetComponent<Node>();

    }

    private void Update()
    {
        UpdateAnimation(isOpened);

        if(thisNode.isStacked)
        {
            PlayerController player = GetComponentInChildren<PlayerController>();
            if(player == null)
            {
                Debug.Log("player is null!!@!");
            }
            
            player.MoveNextStartNode(nextStartNode, nextMovetoNode);

            if(!hasExecuted)
            {
                ExecuteButtonAction();
                hasExecuted = true;  // ������ ���� �� �ٽ� ������� �ʵ��� �÷��� ����
            }

        }

    }


    protected override void ExecuteButtonAction()
    {
        if (sequenceTuples.Count == 0) return;

        if (isSim)
        {
            StartCoroutine(ExecuteSequencesSimulately());
        }
        else
        {
            StartCoroutine(ExecuteSequencesSequentially());
        }
    }


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





    private void UpdateAnimation(bool isOpened)
    {


        if(animator != null)
        {
            animator.SetBool("isOpened", isOpened);
        }
    }
}
