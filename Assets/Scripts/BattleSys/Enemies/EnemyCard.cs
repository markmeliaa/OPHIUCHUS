using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    public string cardName;
    public int life = 20;
    public int speed = 5;
    [HideInInspector] public GameObject nameText;

    private Animator cardAnimator;
    private BattleManager battleManager;

    private void Awake()
    {
        cardAnimator = GetComponent<Animator>();
        battleManager = GameObject.FindGameObjectWithTag("Battle").GetComponent<BattleManager>();
    }

    private void Update()
    {
        if (life <= 0 && !cardAnimator.GetBool("Death"))
            OnDie();
    }

    private void OnDie()
    {
        cardAnimator.SetBool("Death", true);

        battleManager.RedistributeEnemyTexts();
    }
}
