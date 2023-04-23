using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimationMachine
{
    //update trigger anim states
    public static void UpdatePlayerAnim(PlayerAnimState state, Animator anim)
    {
        switch (state)
        {
            default:
                Debug.LogError("PlayerAnimationMachine: Invalid PlayerAnimState");
                return;
        }
    }

    //update bool anim states
    public static void UpdatePlayerAnim(PlayerAnimState state, bool boolean, Animator anim)
    {
        switch (state)
        {
            case PlayerAnimState.IsMoving:
                anim.SetBool("IsMoving", boolean);
                return;

            case PlayerAnimState.IsSprinting:
                anim.SetBool("IsSprinting", boolean);
                return;

            default:
                Debug.LogError("PlayerAnimationMachine: Invalid PlayerAnimState");
                return;
        }
    }
}

public enum PlayerAnimState
{
    IsMoving,
    IsSprinting,
}