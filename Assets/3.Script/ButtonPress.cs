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
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        parentNode = GetComponentInParent<Node>();
        
    }

    private void Update()
    {
        Debug.Log(sequenceTuples[0].target.transform.rotation.eulerAngles);

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

        foreach(SequenceTuple s in sequenceTuples)
        {
            // 시퀀스 실행
            if (s.sequenceManager != null)
            {
                s.sequenceManager.ExecuteSequence(s.target, () => { Debug.Log("Action"); snappingEvent?.Invoke(); });  // 시퀀스 실행
            }
            
        }
    }


    private void UpdateAnimator(bool isPressed)
    {
        animator.SetBool("isPressed", isPressed);
    }

}
