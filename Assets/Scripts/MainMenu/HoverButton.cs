using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour
{
    private Transform transform;
    private Vector3 buttonInitialPos;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        transform = GetComponent<Transform>();

        buttonInitialPos = transform.position;
    }

    public void OnHoverButton()
    {
        if (!GetComponent<Button>().interactable)
        {
            return;
        }

        animator.enabled = true;
        animator.Play("FloatButton"); // Play the animation from the beginning
        animator.SetBool("Hovering", true);
    }

    public void OnStopHoverButton()
    {
        if (!GetComponent<Button>().interactable)
        {
            return;
        }
        
        transform.position = buttonInitialPos; // TODO: Jittering error when placing the cursor on the edge of the button
        animator.enabled = false;
    }
}
