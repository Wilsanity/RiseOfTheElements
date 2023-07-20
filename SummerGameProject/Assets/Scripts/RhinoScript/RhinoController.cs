using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RhinoController : MonoBehaviour
{
    public Animator animator;
    public string attack;

    public Animator chargeAnimator;
    public string chargeAttack;

    private NavMeshAgent agent;
    public Transform player;
    public float chaseSpeed;

    public float attackRadius;
    bool attackReady;

    public Color debugColor;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        attackReady = Vector3.Distance(player.position, transform.position) <= attackRadius;
        animator.SetBool(attack, attackReady);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
