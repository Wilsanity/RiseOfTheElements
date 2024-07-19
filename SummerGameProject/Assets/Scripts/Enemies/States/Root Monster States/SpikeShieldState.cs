using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeShieldState : FSMState
{

    public SpikeShieldState()
    {
        stateType = FSMStateType.Defending;
    }

    public override void Act(Transform player, Transform npc)
    {
        throw new System.NotImplementedException();
    }

    public override void Reason(Transform player, Transform npc)
    {
        throw new System.NotImplementedException();
    }

  
}
