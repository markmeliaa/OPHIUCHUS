using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowArrows : MonoBehaviour
{
    private GameObject arrow;
    private RoomTemplates templates;

    // Start is called before the first frame update
    void Start()
    {
        arrow = gameObject.transform.GetChild(0).gameObject;
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !templates.changingRoom)
            arrow.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !templates.changingRoom)
            arrow.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            arrow.SetActive(false);
    }
}
