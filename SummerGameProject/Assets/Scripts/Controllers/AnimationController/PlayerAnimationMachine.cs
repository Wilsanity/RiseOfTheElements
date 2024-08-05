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
            case PlayerAnimState.IsGrounded:
                _animator.SetBool("IsGrounded", boolean);
                return;
            case PlayerAnimState.ShortDodge:
                _animator.SetTrigger("ShortDodge");
                return;
            case PlayerAnimState.LongDodge:
                _animator.SetTrigger("LongDodge");
                return;
            case PlayerAnimState.IsWolfRunning:
                _animator.SetBool("WolfRunning", boolean);
                return;

            default:
                Debug.LogError("PlayerAnimationMachine: Invalid PlayerAnimState");
                return;
        }
    }
    public void JumpAnimation()
    {
        _animator.SetTrigger("Jump");
    }
}

public enum PlayerAnimState
{
    IsMoving,
    IsSprinting,
    IsGrounded,
    ShortDodge,
    LongDodge,
    IsWolfRunning,
}