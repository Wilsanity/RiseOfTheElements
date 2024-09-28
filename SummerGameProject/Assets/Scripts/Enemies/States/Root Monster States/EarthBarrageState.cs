using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EarthBarrageProjectileState
{
    SPAWN,
    COMPLETE_SPAWN,
    LAUNCH,
    COMPLETE_LAUNCH,
    HIDE_PROJECTILES,
    MOVE_TO_NEXT_STATE,
    MOVE_TO_SHIELD_STATE
}

public class EarthBarrageState : FSMState
{
    private RootMonsterEnemyController _enemyController;

    private EarthBarrageProjectileState _projectileState = EarthBarrageProjectileState.SPAWN;

    private List<GameObject> _projectileGOs = new List<GameObject>();

    private int _projectilesReachedGoal = 0;

    //Properties
    public EarthBarrageProjectileState ProjectileState { get => _projectileState; set => _projectileState = value; }
    public List<GameObject> ProjectileGOs { get => _projectileGOs; set => _projectileGOs = value; }
    public int ProjectilesReachedGoal { get => _projectilesReachedGoal; set => _projectilesReachedGoal = value; }

    public EarthBarrageState(RootMonsterEnemyController rootMonsterController, Animator animator)
    {
        stateType = FSMStateType.Attacking;
        _enemyController = rootMonsterController;
    }


    public override void Act(Transform player, Transform npc)
    {
        CheckIfPlayerInShieldRange(player, npc);

        switch (_projectileState)
        {
            case EarthBarrageProjectileState.SPAWN:
                SpawnProjectileState();
                break;

            case EarthBarrageProjectileState.COMPLETE_SPAWN: 
                break;

            case EarthBarrageProjectileState.LAUNCH:
                LaunchProjectileState();
                break;

            case EarthBarrageProjectileState.COMPLETE_LAUNCH:
                CompleteLaunchState();
                break;

            case EarthBarrageProjectileState.HIDE_PROJECTILES:
                HideProjectilesState();

                break;

            default:
                break;

        }

    }

    private void SpawnProjectileState()
    {
        _projectileState = EarthBarrageProjectileState.COMPLETE_SPAWN;
        _enemyController.SpawnProjectiles(this);

    }

    private void LaunchProjectileState()
    {
        _projectileState = EarthBarrageProjectileState.COMPLETE_LAUNCH;
        _enemyController.LaunchProjectiles(this);
    }

    private void CompleteLaunchState()
    {
        if (_projectilesReachedGoal != _enemyController.BarrageProjectileCount) return;

        ProjectileState = EarthBarrageProjectileState.HIDE_PROJECTILES;
    }

    private void HideProjectilesState()
    {
        _projectilesReachedGoal = 0;
        foreach(GameObject projGO in _projectileGOs)
        {
            projGO.SetActive(false);
        }

        _projectileState = EarthBarrageProjectileState.MOVE_TO_NEXT_STATE;

    }

    private void CheckIfPlayerInShieldRange(Transform player, Transform npc)
    {
        
        //Transition to Spike Shield if the player gets too close
        if (IsInRange(npc, player.position, (int)_enemyController.SpikeShieldRadius))
        {
            ProjectileState = EarthBarrageProjectileState.MOVE_TO_SHIELD_STATE;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {

        //Dead
        if (_enemyController.UnitHealthScript.CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }


        //Transition to Spike Shield if the player gets too close
        if (_projectileState == EarthBarrageProjectileState.MOVE_TO_SHIELD_STATE)
        {
            Debug.Log("TRANSITIONING");
            HideProjectilesState();
            _projectileState = EarthBarrageProjectileState.SPAWN;
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Shield);
        }

        //After the move finishes, we transition to the next state
        if (_projectileState == EarthBarrageProjectileState.MOVE_TO_NEXT_STATE)
        {

            _projectileState = EarthBarrageProjectileState.SPAWN;

            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.AttackOver);
            
        }
    }


   

}

