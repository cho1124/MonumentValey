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


            // 시퀀스가 끝난 뒤에 sequenceCompleted를 true로 설정
            if (s.sequenceManager != null)
            {


                s.sequenceManager.ExecuteSequence(s.target, () =>
                {
                    Debug.Log("Action Completed");
                    snappingEvent?.Invoke();  // 이벤트 호출

                });
            }

            // 시퀀스가 완료될 때까지 대기
            yield return new WaitForSeconds(delayTime);

        }
        

        Debug.Log("All sequences completed.");
    }


}
