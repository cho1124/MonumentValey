using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;

public class GoalPress : CommonPress
{

    private Node goalNode;
    [SerializeField] private bool isPressed;
    [SerializeField] private float delayTime = 1.5f;
  

    protected override void ExecuteButtonAction()
    {
        

        UIManager.instance.GameExit();
    }

    // Start is called before the first frame update

    void Start()
    {
        goalNode = GetComponent<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goalNode.isStacked && !hasExecuted)
        {
            isPressed = true;
            hasExecuted = true;
            ExecuteButtonAction();
        }

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


}
