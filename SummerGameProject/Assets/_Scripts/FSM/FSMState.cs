using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMState
{
    #region Fields
    protected Dictionary<TransitionType, FSMStateType> map = new Dictionary<TransitionType, FSMStateType>();
    protected FSMStateType stateType;
    public FSMStateType ID { get { return stateType; } }
    #endregion

    /// <summary>
    /// Add transition to map
    /// </summary>
    public void AddTransition(TransitionType transition, FSMStateType id)
    {
        // Check if either values entered are null
        if (transition == TransitionType.None || id == FSMStateType.None)
        {
            Debug.LogError("FSMState Error: null Transition/ID.");
            return;
        }

        //Check if the transition exists in the map
        if (map.ContainsKey(transition))
        {
            Debug.LogError("FSMState ERROR: Attempted to add state that already exists in map.");
            return;
        }

        // Add transition and ID to the map
        map.Add(transition, id);
    }

    /// <summary>
    /// Deletes transition from map
    /// </summary>
    public void DeleteTransition(TransitionType transition)
    {
        // Check if value entered is null
        if (transition == TransitionType.None)
        {
            Debug.LogError("FSMState ERROR: null Transition.");
            return;
        }

        // Check if transition exists in map before attempting to remove it
        if (map.ContainsKey(transition))
        {
            map.Remove(transition);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition did not exist in this map.");
    }


    /// <summary>
    /// Returns new state if transition is recieved
    /// </summary>
    public FSMStateType GetOutputState(TransitionType transition)
    {
        // Check if value entered is null
        if (transition == TransitionType.None)
        {
            Debug.LogError("FSMState ERROR: null transition.");
            return FSMStateType.None;
        }

        // Check if transition exists in map
        if (map.ContainsKey(transition))
            return map[transition];

        Debug.LogError("FSMState ERROR: " + transition + " transition did not exist in this map.");
        return FSMStateType.None;
    }

    /// <summary>
    /// Initialize variables when entering state
    /// </summary>
    public virtual void EnterStateInit() { }

    /// <summary>
    /// Decides if state should transition
    /// </summary>
    /// <param name="npc">
    /// Reference to npc controlled by FSMState class. </param>
    public abstract void Reason(Transform player, Transform npc);

    /// <summary>
    /// Controls all NPC behaviour
    /// </summary>
    /// <param name="npc">
    /// Reference to npc controlled by FSMState class. </param>
    public abstract void Act(Transform player, Transform npc);

    /// <summary>
    /// Returns if next position is the same as the current
    /// </summary>
    /// <param name="position">position to check</param>
    protected virtual bool IsInRange(Transform trans, Vector3 position, int range)
    {
        bool inRange = false;
        float distance = Vector3.Distance(trans.position, position);

        if (distance <= range)
            inRange = true;

        return inRange;
    }
}
