using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButton : MonoBehaviour
{
    private Animator buttonAnim;
    public float waitTime = 3.2f;

    // Start is called before the first frame update
    void Start()
    {
        buttonAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        waitTime -= Time.deltaTime;
    }

    public void OnHoverButton()
    {
        if (waitTime > 0)
            return;

        buttonAnim.enabled = true;
        buttonAnim.Play("FloatButton");
        buttonAnim.SetBool("Hovering", true);
    }

    public void OnStopHoverButton()
    {
        if (waitTime > 0)
            return;

        buttonAnim.enabled = false;
    }
}
