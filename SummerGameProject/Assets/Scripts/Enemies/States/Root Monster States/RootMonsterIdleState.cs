using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RootMonsterIdleState : FSMState
{
    private RootMonsterEnemyController _enemyController;
    
    public RootMonsterIdleState (RootMonsterEnemyController enemyController, Animator animator)
    {
        stateType = FSMStateType.Idle;
        _enemyController = enemyController;

    }
    
    public override void Act(Transform player, Transform npc)
    {
        //Do Nothing
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Dead
        if (_enemyController.UnitHealthScript.CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }

        //If they are in a specific range

        if(IsInRange(npc, player.position, (int)_enemyController.SpikeShieldRadius))
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Shield);
        }
        else if (IsInRange(npc, player.position, (int)_enemyController.MidRangeFieldRadius))
        {
            //Prioritize the Root Spears Attack (0.8f) weight
            npc.GetComponent<EnemyController>().PerformTransition(ChooseTransition(RNGVariant(new float[] {0.2f, 0.9f})));
        }
        else if(IsInRange(npc, player.position, (int)_enemyController.FarRangeFieldRadius))
        {
            //Prioritize the Earth Barrage Attack (0.8f) weight
            npc.GetComponent<EnemyController>().PerformTransition(ChooseTransition(RNGVariant(new float[] { 0.9f, 0.2f })));
        }
        
    }

    private TransitionType ChooseTransition(int index)
    {
        if(index == 0)
        {
            return TransitionType.InAttackRange;
        }
        else if(index == 1)
        {
            return TransitionType.InAttack2Range;
        }

        return TransitionType.InAttackRange;
    }

    /// <summary>
    /// 
    /// index 0 = ATTACK 1 = Earth Barrage
    /// index 1 = ATTACK 2 = Root Spears
    /// 
    /// </summary>
    /// <param name="stateAWeight"> On a scale from 0 to 1, the chance to have stateA spawn </param>
    /// <param name="stateBWeight">On a scale from 0 to 1, the chance to have stateB spawn </param>
    /// <returns></returns>
    static int RNGVariant(float[] stateWeights)
    {
        float sumOfProbabilities = 0;
        float prevProbs = 0;

        float[] probs = new float[stateWeights.Length];
        float[] newChance = new float[stateWeights.Length];
        float[] cumulativeProb = new float[stateWeights.Length + 1];

        //Get the sum of all the probabilities
        for (int i = 0; i < probs.Length; i++)
        {
            sumOfProbabilities += stateWeights[i];
        }
            


        //now calculate the new chance of getting each snowball based on the sum
        for (int i = 0; i<probs.Length; i++)
        {
            newChance[i] = stateWeights[i] / sumOfProbabilities;
        }
            


        //now for the cumulativeProbability, aligning them from a range of 0-1
        // s1 (0 - 0.2) s2 (0.2 - 0.4) s3 (0.4 - 0.8)...
        for (int i = 0; i<probs.Length; i++)
        {
            cumulativeProb[0] = 0.00f;

            prevProbs += newChance[i];
            cumulativeProb[i + 1] = prevProbs;
        }


        //now generate a random number between 0-1 and see which cumulativeProb falls into the range. Return the snowball Variant
        float random = Random.Range(0.00f, 1.00f);

        for (int i = 0; i < cumulativeProb.Length; i++)
        {
             if (random > cumulativeProb[i] && random < cumulativeProb[i + 1]) return i;

        }

        return 0;
    }
}
