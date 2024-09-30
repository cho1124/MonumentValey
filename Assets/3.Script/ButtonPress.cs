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
    [Header("시퀀싱 객체쌍")]
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

        // parentNode.isStacked에 따른 버튼 상태 변경
        if (parentNode.isStacked)
        {
            isPressed = true;
        }
        else
        {
            // Multiple 타입의 버튼만 다시 누를 수 있도록 설정
            if (buttonType == ButtonType.Multiple)
            {
                isPressed = false;
                hasExecuted = false;  // Multiple 타입에서는 플래그 초기화
            }
        }

        // 애니메이터 상태 업데이트
        UpdateAnimator(isPressed);

        // isPressed가 true이고 아직 시퀀스가 실행되지 않은 경우 시퀀스 실행
        if (isPressed && !hasExecuted)
        {
            ExecuteButtonAction();
            hasExecuted = true;  // 시퀀스 실행 후 다시 실행되지 않도록 플래그 설정
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

    // 시퀀스를 순차적으로 실행하는 코루틴
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

        Debug.Log("All sequences completed.");
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



    private void UpdateAnimator(bool isPressed)
    {
        animator.SetBool("isPressed", isPressed);
    }

}
