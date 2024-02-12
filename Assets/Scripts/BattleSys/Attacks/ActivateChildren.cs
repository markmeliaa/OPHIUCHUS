using System.Collections;
using UnityEngine;

public class ActivateChildren : MonoBehaviour
{
    public void ActivateMeteos()
    {
        int randomStart = Random.Range(0, 2);
        StartCoroutine(nameof(WaitMeteo), randomStart);
    }

    public void DeactivateMeteos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("FallMeteo", false);
        }

        gameObject.SetActive(false);
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
