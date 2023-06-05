using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//This script is separate from the player controller to prevent any potential merge conflicts
//also I feel like that script has a ton of stuff in it already
public class DodgeController : MonoBehaviour
{
    [SerializeField] private float dodgeForce;
    [SerializeField] private float dodgeCooldown;
    [SerializeField] private Rigidbody physicsBody;
    [SerializeField] private PlayerInput inputActions;
    [SerializeField] private Animator playerAnimator;
    
    private InputAction m_dodgeAction;
    private float m_timeSinceLastDodge;

    private void Awake()
    {
        m_dodgeAction = inputActions.actions["Dodge"];

        m_dodgeAction.performed += ctx =>
        {
            Dodge();
        };
    }

    private void Dodge()
    {
        if (Time.time - m_timeSinceLastDodge < dodgeCooldown)
            return; //In the future display something so the player knows they need to wait to dodge
        
        playerAnimator.SetTrigger("Dodge");
        
        physicsBody.AddForce(transform.forward * dodgeForce, ForceMode.Impulse);
        m_timeSinceLastDodge = Time.time;
    }
}
