using UnityEngine;

public class HideTilemap : MonoBehaviour
{
    private Renderer colliderRenderer;

    private void Start()
    {
        colliderRenderer = GetComponent<Renderer>();
        colliderRenderer.enabled = false;
    }
}
