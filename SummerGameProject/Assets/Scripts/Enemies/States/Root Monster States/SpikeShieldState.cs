using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpikeShieldState : FSMState
{
    private enum ShieldState
    {
        SPAWN,
        COMPLETED_SPAWN,
        DESPAWN,
        MOVE_TO_NEXT_STATE
    }

    private ShieldState _shieldState = ShieldState.SPAWN;

    private float _duration;
    private float _originalTime;
    private RootMonsterEnemyController _enemyController;
    private GameObject shieldGO;


    public SpikeShieldState(RootMonsterEnemyController enemyController, float duration)
    {
        stateType = FSMStateType.Defending;
        _enemyController = enemyController;
        _duration = duration;   
    }

    public override void Act(Transform player, Transform npc)
    {

        switch(_shieldState)
        {
            case ShieldState.SPAWN:
                SpawnShield(npc);
                break;

            case ShieldState.COMPLETED_SPAWN:
                WaitForShieldDuration();
                break;

            case ShieldState.DESPAWN:
                DespawnShield();
                break;

            default:
                break;
        }
       

        //If the gameObject hasn't been instantiated yet, instantiate it

        //Wait For blank seconds

        //Reason to exit state
    }

    public override void EnterStateInit()
    {
        _originalTime = Time.time;
    }
    private void SpawnShield(Transform npc)
    {
        if (shieldGO == null)
        {
            shieldGO = _enemyController.SpawnGameObject(_enemyController.SpikeShiledPrefab, npc.position);
            shieldGO.SetActive(true);
        }
        else
        {
            shieldGO.SetActive(true);
            shieldGO.transform.position = npc.position;
        }

        _shieldState = ShieldState.COMPLETED_SPAWN;

    }

    private void WaitForShieldDuration()
    {
        if (Time.time < _originalTime + _duration) return;

        _shieldState = ShieldState.DESPAWN;
    }

    private void DespawnShield()
    {
        shieldGO.SetActive(false);
        _shieldState = ShieldState.MOVE_TO_NEXT_STATE;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Dead
        if (_enemyController.UnitHealthScript.CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }

        //Run Away From Player
        if (_shieldState == ShieldState.MOVE_TO_NEXT_STATE)
        {
            _shieldState = ShieldState.SPAWN;


            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.AttackOver);
        }

        

    }

  
}
