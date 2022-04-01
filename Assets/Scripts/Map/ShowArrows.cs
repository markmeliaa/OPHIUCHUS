using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowArrows : MonoBehaviour
{
    private GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        arrow = gameObject.transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            arrow.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            arrow.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            arrow.SetActive(false);
    }
}
