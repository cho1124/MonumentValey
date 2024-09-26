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
            isPressed = !isPressed;
        }
        else
        {
            if(buttonType == ButtonType.Multiple)
            {
                isPressed = !isPressed;
            }
        }

        UpdateAnimator(isPressed);

    }

    private void UpdateAnimator(bool isPressed)
    {
        animator.SetBool("isPressed", isPressed);
    }

}
