using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;

public enum ButtonType
{
    Once,
    Multiple
}

public class ButtonPress : MonoBehaviour
{
    [SerializeField] private ButtonType buttonType = ButtonType.Once;
    [SerializeField] private bool isPressed = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Node parentNode;
    [Header("영향받는 오브젝트")]
    [SerializeField] private List<Transform> MoveTransform;

    public ButtonType ButtonType => buttonType;
    public bool IsPressed => isPressed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        parentNode = GetComponentInParent<Node>();
    }


    private void Update()
    {
        if (buttonType == ButtonType.Once && isPressed) return;

        if(parentNode.isStacked)
        {
            isPressed = true;
        }
        else
        {
            if(buttonType == ButtonType.Multiple)
            {
                isPressed = false;
            }
        }

        UpdateAnimator(isPressed);

    }

    private void UpdateAnimator(bool isPressed)
    {
        animator.SetBool("isPressed", isPressed);
    }

}
