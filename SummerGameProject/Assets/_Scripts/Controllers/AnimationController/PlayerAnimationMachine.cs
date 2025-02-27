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

    //update bool anim states
    public void UpdatePlayerAnim(PlayerAnimState state, bool boolean = true)
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
                _animator.SetBool("GoingIntoLongDodge", boolean);
                return;
            case PlayerAnimState.IsWolfRunning:
                _animator.SetBool("WolfRunning", boolean);
                return;
            case PlayerAnimState.AirDodge:
                _animator.SetTrigger("AirDodge");
                return;

            case PlayerAnimState.BasicAttack:
                _animator.SetTrigger("Attack");
                return;
            case PlayerAnimState.ComboAttack:
                _animator.SetBool("GoingIntoComboAttack", boolean);
                return;

            case PlayerAnimState.Hit:
                _animator.SetTrigger("Hit");
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
        VisualEffect tmp = Instantiate(visualEffect, rightFoot.position, Quaternion.identity);
        Destroy(tmp.gameObject, 1.2f);
    }
    public void KickUpDustLeft(VisualEffect visualEffect)
    {
        VisualEffect tmp = Instantiate(visualEffect, leftFoot.position, Quaternion.identity);
        Destroy(tmp.gameObject, 1.2f);
    }
}



public enum PlayerAnimState
{
    IsMoving,
    IsSprinting,
    IsGrounded,
    ShortDodge,
    LongDodge,
    AirDodge,
    IsWolfRunning,
    BasicAttack,
    ComboAttack,
    Hit,
}