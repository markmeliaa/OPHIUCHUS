using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private ButtonManager buttonManager;

    private void Awake()
    {
        buttonManager = GameObject.FindGameObjectWithTag("Buttons").GetComponent<ButtonManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //TriggerEnemyBattle();
        }
    }

    void TriggerEnemyBattle()
    {
        buttonManager.StartBattle();
        GetComponent<CircleCollider2D>().enabled = false;
    }
}
