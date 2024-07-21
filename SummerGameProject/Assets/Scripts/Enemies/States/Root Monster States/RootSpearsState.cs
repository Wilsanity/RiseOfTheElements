using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum RootSpearsProjectileState
{
    INITIAL_DELAY,
    SPAWN,
    SPAWNING,
    COMPLETE_SPAWN,
    RETRACT_SPEARS,
    HIDE_SPEARS,
    MOVE_TO_NEXT_STATE
}

public class RootSpearsState : FSMState
{
    private RootMonsterEnemyController _enemyController;

    private RootSpearsProjectileState _rootState = RootSpearsProjectileState.SPAWN;

    private List<GameObject> _projectileGOs = new List<GameObject>();

    private int _spikesRetracted = 0;

    private float _originalTime;

    private bool _rootIsHit = false;

    //Properties
    public RootSpearsProjectileState ProjectileState { get => _rootState; set => _rootState = value; }
    public List<GameObject> ProjectileGOs { get => _projectileGOs; set => _projectileGOs = value; }
    public int SpikesRetracted { get => _spikesRetracted; set => _spikesRetracted = value; }
    public float OriginalTime { set => _originalTime = value; }
    public bool RootIsHit { get => _rootIsHit; set => _rootIsHit = value; }

    public RootSpearsState(RootMonsterEnemyController rootMonsterController, Animator animator)
    {
        stateType = FSMStateType.RootSpearAttack;
        _enemyController = rootMonsterController;
    }


    public override void Act(Transform player, Transform npc)
    {
        switch (_rootState)
        {
            case RootSpearsProjectileState.INITIAL_DELAY:
                InitialDelayState();
                break;

            case RootSpearsProjectileState.SPAWN:
                SpawnRootSpearState();
                break;

            case RootSpearsProjectileState.SPAWNING:
                SpawningState();
                break;

            case RootSpearsProjectileState.COMPLETE_SPAWN:
                CompleteSpawnState();
                break;

            case RootSpearsProjectileState.RETRACT_SPEARS:
                RetractSpearState();
                break;

            case RootSpearsProjectileState.HIDE_SPEARS:
                HideProjectilesState();
                break;

            default:
                break;

        }

    }

    private void InitialDelayState()
    {
        _originalTime = Time.time;
    }
    private void SpawnRootSpearState()
    {
 
        if (Time.time < _originalTime + _enemyController.RootSpearInitialDelay) return;

        _rootState = RootSpearsProjectileState.SPAWNING;
        _enemyController.SpawnRootSpears(this);

    }


    private void SpawningState()
    {
        //Do nothing
    }

    private void CompleteSpawnState()
    {
        //The next state will be activated if time runs out OR a root spear gets hit by the player

        if (_rootIsHit)
        {
            _rootState = RootSpearsProjectileState.RETRACT_SPEARS;
        }



        if (Time.time < _originalTime + _enemyController.RootSpearAttackDuration) return;
       

        _rootState = RootSpearsProjectileState.RETRACT_SPEARS;
     

    }

    private void RetractSpearState()
    {
        
        foreach(GameObject go in _projectileGOs)
        {
            go.GetComponent<RootSpearProjectile>().SpikeDown();
        }

        if(_spikesRetracted == _projectileGOs.Count)
        {
            _rootState = RootSpearsProjectileState.HIDE_SPEARS;
        }

        
    }

   

    private void HideProjectilesState()
    {
        _spikesRetracted = 0;
        foreach (GameObject projGO in _projectileGOs)
        {
            projGO.SetActive(false);
        }

        _rootState = RootSpearsProjectileState.MOVE_TO_NEXT_STATE;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Dead
        if (_enemyController.UnitHealthScript.CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }

        //After the move finishes, we transition to the next state

        if (_rootState == RootSpearsProjectileState.MOVE_TO_NEXT_STATE)
        {
            _rootState = RootSpearsProjectileState.SPAWN;
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.AttackOver);
        }
    }




}
