using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAnimationMachine : MonoBehaviour
{

    private PlayerMovement _movement;
    private PlayerController _controller;
    private Animator _animator;
    [SerializeField]
    Transform leftFoot, rightFoot;

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
            case PlayerAnimState.IsGrounded:
                _animator.SetBool("IsGrounded", boolean);
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

    public void KickUpDustRight(VisualEffect visualEffect)
    {
        Instantiate(visualEffect, rightFoot.position, Quaternion.identity);
        Destroy(visualEffect, 1.2f);
    }
    public void KickUpDustLeft(VisualEffect visualEffect)
    {
        Instantiate(visualEffect, leftFoot.position, Quaternion.identity);
        Destroy(visualEffect, 1.2f);
    }
}



public enum PlayerAnimState
{
    IsMoving,
    IsSprinting,
    IsGrounded
}