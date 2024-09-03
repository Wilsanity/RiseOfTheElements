using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSMStateType
{
    None = 0,
    Idle,
    Wandering,
    Chasing,
    Attacking,
    MovingAway,
    Cooldown,
    Defending,
    TakingDamage,
    Dead,

    //Root Monster States
    RootSpearAttack,

    //Poison Frog States
    BounceAttacking,
    BounceBombAttacking,
    TronRollAttacking,

    // Beetle States
    MucusAttacking,
    FlyingArrowAttacking,
    HornSwipeAttacking
}

public enum TransitionType
{
    None = 0,
    EnterIdle,
    OutOfRange,
    PlayerInRange,
    InChaseRange,
    InAttackRange,
    InAttack2Range,
    InAttack3Range,
    AttackOver,
    DamageTaken,
    Cooldown,
    Hit,
    NoHealth,

    //Root Monster Specific Transitions
    Shield,

    //Poison Frog Transitions
    InBounceRange,
    InBounceBombRange,
    InTronRollRange,

    // Beetle Transitions
    InTauntMode,
    InAggroMode,
    InMeleeRange,
    OutOfMeleeRange

}

public class AdvancedFSM : FSM
{
    #region Fields
    private List<FSMState> states;

    private FSMStateType currentStateType;
    public FSMStateType CurrentStateType { get { return currentStateType; } }

    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }
    #endregion

    public AdvancedFSM()
    {
        states = new List<FSMState>();
    }

    /// <summary>
    /// Add state to list
    /// </summary>
    /// <param name="stateToAdd">
    /// State you want to add. </param>
    public void AddState(FSMState stateToAdd)
    {
        // Check if value entered is null
        if(stateToAdd == null)
            Debug.LogError("AdvancedFSM Error: State being added was null.");

        // Check if this is the first state (initial state when games starts)
        if (states.Count == 0)
        {
            states.Add(stateToAdd);
            currentState = stateToAdd;
            currentStateType = stateToAdd.ID;
            return;
        }

        // Add state to list
        foreach(FSMState s in states)
        {
            // Check if state already exists
            if(s.ID == stateToAdd.ID)
            {
                Debug.LogError("AdvancedFSM Error: Attempted to add state that already exists.");
                return;
            }
        }
        states.Add(stateToAdd);
    }

    /// <summary>
    /// Deletes state from list
    /// </summary>
    /// <param name="stateTypeToDelete"></param>
    public void DeleteState(FSMStateType stateTypeToDelete)
    {
        // Check if value entered is null
        if(stateTypeToDelete == FSMStateType.None)
        {
            Debug.LogError("AdvancedFSM Error: State being deleted was null.");
            return;
        }

        // Check if state already exists
        foreach(FSMState s in states)
        {
            // Found state in list
            if(s.ID == stateTypeToDelete)
            {
                // Remove from state list
                states.Remove(s);
                return;
            }
        }
        Debug.LogError("AdvancedFSM Error: Attempted to delete state that does not exist on list.");
    }

    /// <summary>
    /// Attempts to change state based on transition passed
    /// </summary>
    /// <param name="transition"></param>
    public void PerformTransition(TransitionType transition)
    {
        // Check if value entered is null
        if (transition == TransitionType.None)
        {
            Debug.LogError("AdvancedFSM Error: Transition was null.");
            return;
        }

        FSMStateType id = currentState.GetOutputState(transition);
        if (id == FSMStateType.None)
        {
            Debug.LogError("AdvancedFSM Error: currentState does not have a target state for this transition.");
            return;
        }

        // Update currentStateType and currentState
        currentStateType = id;
        foreach(FSMState s in states)
        {
            if(s.ID == currentStateType)
            {
                currentState = s;
                currentState.EnterStateInit();
                break;
            }
        }
    }
}
