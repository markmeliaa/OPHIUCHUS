using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private BattleInputManager battleInputManager;

    private void Awake()
    {
        battleInputManager = GameObject.FindGameObjectWithTag("BattleMngr").GetComponent<BattleInputManager>();
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
        battleInputManager.StartBattle();
        GetComponent<CircleCollider2D>().enabled = false;
    }
}
