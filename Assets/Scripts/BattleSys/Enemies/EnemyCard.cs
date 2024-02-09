using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    public string CardName { get; private set; }
    public int Life { get; set; } = 20;

    //[HideInInspector] public GameObject nameText { private get; set; }

    private Animator cardAnimator;
    private BattleManager battleManager;

    private void Awake()
    {
        cardAnimator = GetComponent<Animator>();
        battleManager = GameObject.FindGameObjectWithTag("Battle").GetComponent<BattleManager>();
    }

    private void Update()
    {
        // TODO: Ideally, this would not need an Update and it should be checked in the damage script
        if (Life <= 0 && !cardAnimator.GetBool("Death"))
        {
            ManageCardDeath();
        }
    }

    private void ManageCardDeath()
    {
        cardAnimator.SetBool("Death", true);
        battleManager.RedistributeEnemyTexts();
    }
}
