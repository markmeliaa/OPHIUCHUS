using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHeightLvl : MonoBehaviour
{
    public float height = 0.0f;
    public Collider2D[] enableColl;
    public Collider2D[] disableColl;

    private void OnTriggerExit2D(Collider2D collider)
    {
        GameObject go = collider.gameObject;
        if (go == null || !collider.gameObject.CompareTag("Player") || go.transform.parent.position.z == height)
            return;

        Debug.Log("a");
        if (enableColl != null)
        {
            foreach (var coll in enableColl)
                collider.gameObject.SetActive(true);
        }

        if (disableColl != null)
        {
            foreach (var coll in disableColl)
                collider.gameObject.SetActive(false);
        }

        Vector3 position = go.transform.position;
        Vector3 newPosition = new Vector3(position.x, position.y, height);
        go.transform.parent.position = newPosition;
    }
}
