using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverEffectButton : MonoBehaviour
{
    private Button buttonComp;
    private Animator animatorComp;

    private Vector3 buttonInitialPos;

    void Start()
    {
        animatorComp = GetComponent<Animator>();
        buttonComp = GetComponent<Button>();

        buttonInitialPos = transform.position;
    }

    public void OnHoverButton()
    {
        if (buttonComp.interactable)
        {
            return;
        }

        animatorComp.enabled = true;
        animatorComp.Play("FloatButton"); // Play this animation from the beginning
        animatorComp.SetBool("Hovering", true);
    }

    public void OnStopHoverButton()
    {
        if (buttonComp.interactable)
        {
            return;
        }
        
        transform.position = new Vector2(transform.position.x, buttonInitialPos.y);
        animatorComp.enabled = false;

        // TODO: Fix jittering error when placing the cursor on the edge of the button
    }
}
