using UnityEngine;

public class DealDamageToPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameMaster.playerLife > 2)
            {
                GameMaster.playerLife -= 2;
            }
            else
            {
                GameMaster.playerLife = 0;
            }

            collision.gameObject.GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponent<Animator>().SetBool("Hit", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Animator>().SetBool("Hit", false);
        }
    }
}
