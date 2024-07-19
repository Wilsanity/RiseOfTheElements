using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RootMonsterEnemyController : EnemyController
{

    [Header("Ranges")]
    [SerializeField]
    [Tooltip("Wander Radius")]
    private float _wanderRadius;

    [SerializeField]
    [Tooltip("The Range where the Root Monster prioritizes the Earth Barrage attack")]
    private float _farRangeFieldRadius;

    [SerializeField]
    [Tooltip("The Range where the Root Monster prioritizes the Root Spears attack")]
    private float _midRangeFieldRadius;

    [SerializeField]
    [Tooltip("The Range where the Root Monster will defend itself")]
    private float _spikeShieldRadius;

    protected override void ConstructFSM()
    {
        //Constructing the Enemy States
        WanderState wanderState = new WanderState(speed, _wanderRadius, animator);

        CooldownState cooldownState = new CooldownState(2, animator);

        EarthBarrageState earthBarrageState = new EarthBarrageState();

        RootSpearsState rootSpearsState = new RootSpearsState();

        SpikeShieldState spikeShieldState = new SpikeShieldState();

        RunFromPlayerState runFromPlayerState = new RunFromPlayerState();

        DeadState deadState = new DeadState(animator);


        //Adding Transitions
        //Wander To Earth Barrage
        wanderState.AddTransition(TransitionType.InAttackRange, FSMStateType.Attacking);

        //Wander to Root Spears
        wanderState.AddTransition(TransitionType.InAttack2Range, FSMStateType.Attacking);

        //Wander to Spike Shield
        wanderState.AddTransition(TransitionType.InAttack3Range, FSMStateType.Defending);

        //Wander to Death
        wanderState.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        //Earth Barrage to Wander
        earthBarrageState.AddTransition(TransitionType.Hit, FSMStateType.Cooldown);
        earthBarrageState.AddTransition(TransitionType.AttackOver, FSMStateType.Cooldown);

        //Root Spears to Wander
        rootSpearsState.AddTransition(TransitionType.Hit, FSMStateType.Cooldown);
        rootSpearsState.AddTransition(TransitionType.AttackOver, FSMStateType.Cooldown);

        //Cooldown State
        cooldownState.AddTransition(TransitionType.InAttackRange, FSMStateType.Wandering);

        //Spike Shield to Running from player
        spikeShieldState.AddTransition(TransitionType.AttackOver, FSMStateType.MovingAway);
        runFromPlayerState.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);
        runFromPlayerState.AddTransition(TransitionType.PlayerInRange, FSMStateType.Wandering);




        //Adding the States
        AddState(wanderState);
        AddState(earthBarrageState);
        AddState(rootSpearsState);
        AddState(spikeShieldState);
        AddState(runFromPlayerState);
        AddState(deadState);
    }

    private void OnDrawGizmos()
    {
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 14;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;

        // Enter Range
        Gizmos.color = Color.green;
        textStyle.normal.textColor = Color.green;
        Gizmos.DrawWireSphere(transform.position, ENTER_RANGE);
#if UNITY_EDITOR
        Handles.Label(transform.position + transform.forward * ENTER_RANGE, "ENTER RANGE Radius", textStyle);
#endif


        // Exit Range
        Gizmos.color = Color.red;
        textStyle.normal.textColor = Color.red;
        Gizmos.DrawWireSphere(transform.position, EXIT_RANGE);
#if UNITY_EDITOR
        Handles.Label(transform.position + transform.forward * EXIT_RANGE, "EXIT RANGE Radius", textStyle);
#endif


        // Wander Radius
        Handles.color = Color.white;
        textStyle.normal.textColor = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, _wanderRadius, 3);
#if UNITY_EDITOR
        Handles.Label(transform.position + transform.forward * _wanderRadius, "Wander Radius", textStyle);
#endif


        // Far Range Attack
        Handles.color = Color.magenta;
        textStyle.normal.textColor = Color.magenta;
        Handles.DrawWireDisc(transform.position, Vector3.up, _farRangeFieldRadius, 3);
#if UNITY_EDITOR
        Handles.Label(transform.position + transform.forward * _farRangeFieldRadius, "Far Range Radius", textStyle);
#endif


        // Mid Range Attack
        Handles.color = Color.yellow;
        textStyle.normal.textColor = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, _midRangeFieldRadius, 3);
#if UNITY_EDITOR
        Handles.Label(transform.position + transform.forward * _midRangeFieldRadius, "Mid Range Radius", textStyle);
#endif


        // Shield Attack
        Handles.color = Color.cyan;
        textStyle.normal.textColor = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, _spikeShieldRadius, 3);
        #if UNITY_EDITOR
        Handles.Label(transform.position  + transform.forward * _spikeShieldRadius, "Spike Shield Radius", textStyle);
        #endif
    }
}
