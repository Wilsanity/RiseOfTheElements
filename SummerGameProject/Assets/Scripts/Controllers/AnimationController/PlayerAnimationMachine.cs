using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationMachine : MonoBehaviour
{

    private PlayerMovement _movement;
    private PlayerController _controller;
    private Animator _animator;
    //update trigger anim states

    private void Awake()
    {
        _animator = GetComponent<Animator>();   
    }
    public void UpdatePlayerAnim(PlayerAnimState state, Animator anim)
    {
        switch (state)
        {
            default:
                Debug.LogError("PlayerAnimationMachine: Invalid PlayerAnimState");
                return;
        }
    }

    //update bool anim states
    public void UpdatePlayerAnim(PlayerAnimState state, bool boolean)
    {
        switch (state)
        {
            case PlayerAnimState.IsMoving:
                _animator.SetBool("IsMoving", boolean);
                return;

            case PlayerAnimState.IsSprinting:
                _animator.SetBool("IsSprinting", boolean);
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