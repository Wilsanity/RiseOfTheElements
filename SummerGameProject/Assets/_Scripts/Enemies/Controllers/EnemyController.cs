using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// Basic enemy controller
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyController : AdvancedFSM, IDamageable
{
    [Header("State Debug Info")]
    [Tooltip("Enemy's State: Debug purposes only.")] [SerializeField] private string stateDebug;

    [Header("Agro Ranges")]
    [Tooltip("Range enemy begins chasing (increases to increase agro).")] public int ENTER_RANGE;
    [Tooltip("Range enemy stops chasing (keep this above enter range).")] public int EXIT_RANGE;

    public Transform player;
    public Animator animator;

    [Header("Base Variables")]
    ///[Tooltip("Max health of enemy.")]
    ///public int maxHealth;
    ///[Tooltip("Current health of enemy.")]
    ///public float health;
    [Tooltip("Speed enemy moves.")]
    public int speed;
    [Tooltip("Amount of damage applied to player")]
    public int damage;

    public Text debugStateText;

    /// <summary>
    /// Returns enemy health
    /// </summary>
    ///public float GetHealth() { return health; }

    //public void TakeDamage(int damageAmt)
    //{
    //    //if(health > 0)
    //    //{
    //    //    if (damageAmt > health)
    //    //        health = 0;
    //    //    else
    //    //        health -= damage;
    //    //}

    //    GetComponent<UnitHealth>().DamageUnit(damageAmt);
    //}

    /// <summary>
    /// Returns CurrentState as a string
    /// </summary>
    public string GetStringState() { return CurrentState.ID.ToString(); }

    protected override void Initialize()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;

        // Store reference to player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        //TO DO: Implement Animations

        // Set health value
        //health = maxHealth;
        GetComponent<UnitHealth>().CurrentHealth = GetComponent<UnitHealth>().MaxHealth;

        ConstructFSM();
    }

    protected override void FSMUpdate()
    {
        elapsedTime += Time.deltaTime;
        CurrentState.Reason(player, transform);
        CurrentState.Act(player, transform);
        stateDebug = GetStringState();
        ///debugStateText.text = GetStringState();
    }

    protected abstract void ConstructFSM();




    //Interface definition.
    //Ideally this isn't in every monster class....
    public void TakeDamage()
    {

        Debug.Log(this.gameObject.name +  ", I'm receiving damage!");



    }

    public void DealDamage()
    {

    }



}
