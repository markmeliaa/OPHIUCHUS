using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    public string CardName { get; private set; }
    public int Life { get; set; }

    //[HideInInspector] public GameObject nameText { private get; set; }

    [SerializeField] private string thisCardName;
    [SerializeField] private int thisCardLife = 20;

    private Animator cardAnimator;
    private BattleActionsManager battleActionsManager;

    private void Awake()
    {
        CardName = thisCardName;
        Life = thisCardLife;

        cardAnimator = GetComponent<Animator>();
        battleActionsManager = GameObject.FindGameObjectWithTag("BattleMngr").GetComponent<BattleActionsManager>();
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
        battleActionsManager.UpdateEnemiesInBattle();
    }
}
