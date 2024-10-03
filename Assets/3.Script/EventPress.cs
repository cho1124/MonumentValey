using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;

public class EventPress : CommonPress
{
    private Node thisNode;
    [SerializeField] private bool isSim = false;
    [SerializeField] private float delayTime = 0.5f;
    


    private void Start()
    {
        thisNode = GetComponent<Node>();

    }

    private void Update()
    {
        
        if (thisNode.isStacked)
        {
            PlayerController player = GetComponentInChildren<PlayerController>();
            if (player == null)
            {
                Debug.Log("player is null!!@!");
            }


            if (!hasExecuted)
            {
                snappingEvent?.Invoke();
                ExecuteButtonAction();
                hasExecuted = true;  // 시퀀스 실행 후 다시 실행되지 않도록 플래그 설정
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
    // Start is called before the first frame update
   

    private IEnumerator ExecuteSequencesSequentially()
    {
        foreach (SequenceTuple s in sequenceTuples)
        {
            bool sequenceCompleted = false;

            // 시퀀스가 끝난 뒤에 sequenceCompleted를 true로 설정
            if (s.sequenceManager != null)
            {
                s.sequenceManager.ExecuteSequence(s.target, () =>
                {
                    Debug.Log("Action Completed");
                    snappingEvent?.Invoke();  // 이벤트 호출
                    sequenceCompleted = true;  // 시퀀스 완료
                });
            }

            // 시퀀스가 완료될 때까지 대기
            yield return new WaitUntil(() => sequenceCompleted);
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


        
    }
}
