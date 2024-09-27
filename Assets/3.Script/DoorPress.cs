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
            
            player.MoveNextStartNode(nextStartNode);

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

        foreach (SequenceTuple s in sequenceTuples)
        {
            // ������ ����
            if (s.sequenceManager != null)
            {
                s.sequenceManager.ExecuteSequence(s.target, () => { Debug.Log("Action"); snappingEvent?.Invoke(); });  // ������ ����
            }

        }
    }



    private void UpdateAnimation(bool isOpened)
    {


        if(animator != null)
        {
            animator.SetBool("isOpened", isOpened);
        }
    }
}
