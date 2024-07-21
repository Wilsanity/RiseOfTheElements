using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum RootSpearsProjectileState
{
    INITIAL_DELAY,
    SPAWN,
    COMPLETE_SPAWN,
    RETRACT_SPEARS,
    HIDE_SPEARS,
    MOVE_TO_NEXT_STATE
}

public class RootSpearsState : FSMState
{
    private RootMonsterEnemyController _rootMonsterController;

    private RootSpearsProjectileState _rootState = RootSpearsProjectileState.SPAWN;

    private List<GameObject> _projectileGOs = new List<GameObject>();

    private int _projectilesReachedGoal = 0;

    private float _originalTime;


    //Properties
    public RootSpearsProjectileState ProjectileState { get => _rootState; set => _rootState = value; }
    public List<GameObject> ProjectileGOs { get => _projectileGOs; set => _projectileGOs = value; }
    public int ProjectilesReachedGoal { get => _projectilesReachedGoal; set => _projectilesReachedGoal = value; }

    public RootSpearsState(RootMonsterEnemyController rootMonsterController, Animator animator)
    {
        stateType = FSMStateType.Attacking;
        _rootMonsterController = rootMonsterController;
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
        if (Time.time < _originalTime + _rootMonsterController.RootSpearInitialDelay) return;

        _rootState = RootSpearsProjectileState.COMPLETE_SPAWN;
        _rootMonsterController.SpawnRootSpears(this);

    }

    private void CompleteSpawnState()
    {
        if (_projectilesReachedGoal != _rootMonsterController.BarrageProjectileCount) return;

        ProjectileState = RootSpearsProjectileState.RETRACT_SPEARS;
    }

    private void RetractSpearState()
    {
        _rootState = RootSpearsProjectileState.HIDE_SPEARS;
        //_rootMonsterController.LaunchProjectiles(this);
    }

   

    private void HideProjectilesState()
    {
        _projectilesReachedGoal = 0;
        foreach (GameObject projGO in _projectileGOs)
        {
            projGO.SetActive(false);
        }

        _rootState = RootSpearsProjectileState.MOVE_TO_NEXT_STATE;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //After the move finishes, we transition to the next state

        if (_rootState == RootSpearsProjectileState.MOVE_TO_NEXT_STATE)
        {
            _rootState = RootSpearsProjectileState.SPAWN;
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.AttackOver);
        }
    }




}
