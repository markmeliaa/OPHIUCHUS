using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    public string cardName;
    public int vida = 0;
    public int speed = 5;

    private Animator cardAnimator;

    private void Start()
    {
        cardAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (vida <= 0)
            OnDie();
    }

    private void OnDie()
    {
        cardAnimator.SetBool("Death", true);
    }
}
