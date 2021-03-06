using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateChilds : MonoBehaviour
{
    public void ActivateMeteos()
    {
        int randomStart = Random.Range(0, 2);
        StartCoroutine("WaitMeteo", randomStart);
    }

    public void DeactivateMeteos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("FallMeteo", false);
        }
    }

    IEnumerator WaitMeteo(int start)
    {
        if (start == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Animator>().SetBool("FallMeteo", true);
                yield return new WaitForSeconds(1.5f);
            }
        }
        
        else
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).GetComponent<Animator>().SetBool("FallMeteo", true);
                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}
