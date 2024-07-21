using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RootMonsterEnemyController : EnemyController
{

    [Space]
    [Title("Ranges", TextAlignment.Left, TextColour.White, 20)]
    [Separator]

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


    #region Earth Barrage Variables
    [Space]
    [Title("Earth Barrage Attack",TextAlignment.Left,TextColour.White,20)]
    [Separator]

    [Header("Spawning Projectiles")]
    [SerializeField]
    [Tooltip("The projectile Game Object that should be thrown using this attack")]
    private GameObject _barrageProjectilePrefab;

    [SerializeField]
    [Tooltip("Number of Earth Barrage Projectiles to Spawn")]
    [Range(1,10)]
    private int _barrageProjectileCount;

    [SerializeField]
    [Tooltip("The delay in seconds from spawning one projectile to the next. \n Spawn, Delay, Spawn, Delay...")]
    [Range(0, 1)]
    private float _barrageProjectileSpawnDelay;

    [SerializeField]
    [Tooltip("The delay in seconds after we finish spawning all the projectiles")]
    [Range(0, 3)]
    private float _barrageProjectileWaitTimeAfterSpawn;

    [Header("Launching Projectiles")]
    [SerializeField]
    [Tooltip("The delay in seconds from launching one projectile to the next. \n Launch, Delay, Launch, Delay...")]
    [Range(0, 1)]
    private float _barrageProjectileLaunchDelay;

    [SerializeField]
    [Tooltip("The Speed the Earth Barrage Projectiles Spawn")]
    private float _barrageProjectileSpeed;

    [SerializeField]
    [Tooltip("When launching the projectiles, how much should the projectiles arc? \n 0 = straight line, 30 = arched curve")]
    [Range(0,30)]
    private float _barrageProjectileMaxArc;

    [SerializeField]
    [Tooltip("The projectiles home in on the player's previous position, but how precise does that position have to be? \n" +
        "We can make the projectiles have a sort of random output for their desination")]
    private float _barrageDestinationOffsetRadius;

    #endregion


    #region Root Spears Variables
    [Space]
    [Title("Root Spears Attack", TextAlignment.Left, TextColour.White, 20)]
    [Separator]


    [Header("Spawning Spears")]
    [SerializeField]
    [Tooltip("The Root Spear Game Object that should be thrown using this attack")]
    private GameObject _rootSpearPrefab;

    [SerializeField]
    [Tooltip("Delay, in seconds, for the aniticipation of starting this attack (spawning the root spears)")]
    [Range(0, 2)]
    private float _rootSpearInitialDelay;

    [SerializeField]
    [Tooltip("Minimum Number of Root Spears to Spawn")]
    [Range(1, 10)]
    private int _rootSpearCountMin;

    [SerializeField]
    [Tooltip("Maximum Number of Root Spears to Spawn")]
    [Range(1, 10)]
    private int _rootSpearCountMax;

    [SerializeField]
    [Tooltip("The delay in seconds from spawning one root spear to the next. \n Spawn, Delay, Spawn, Delay...")]
    [Range(0, 1)]
    private float _rootSpearSpawnDelay;

    [Header("Launching Spears")]
    [SerializeField]
    [Tooltip("The Minimum delay in seconds from launching a root spear from the ground")]
    private float _rootSpearPopUpDelayMin;

    [SerializeField]
    [Tooltip("The Maximum delay in seconds from launching a root spear from the ground")]
    private float _rootSpearPopUpDelayMax;

    [SerializeField]
    [Tooltip("The Speed the Root Spears to Pop Up from the ground")]
    private float _rootSpearPopUpSpeed;

    [SerializeField]
    [Tooltip("The area around the player to launch")]
    private float _rootRadiusAroundPlayer;
    #endregion



    //Earth Barrage Properties
    public GameObject EarthBarrageProjectilePrefab { get => _barrageProjectilePrefab; }
    public int BarrageProjectileCount { get => _barrageProjectileCount; }
    public float BarrageProjectileSpeed { get => _barrageProjectileSpeed; }
    public float BarrageProjectileMaxArc { get => _barrageProjectileMaxArc; }
    public float BarrageDestinationOffsetRadius { get => _barrageDestinationOffsetRadius; }

    //Root Spear Properties
    public float RootSpearInitialDelay { get => _rootSpearInitialDelay; }
    public GameObject RootSpearPrefab { get => _rootSpearPrefab;}
    public int RootSpearCountMin { get => _rootSpearCountMin;}
    public int RootSpearCountMax { get => _rootSpearCountMax; }
    public float RootSpearSpawnDelay { get => _rootSpearSpawnDelay; }
    public float RootSpearPopUpDelayMin { get => _rootSpearPopUpDelayMin; }
    public float RootSpearPopUpDelayMax { get => _rootSpearPopUpDelayMax; }
    public float RootSpearPopUpSpeed { get => _rootSpearPopUpSpeed; }
    public float RootRadiusAroundPlayer { get => _rootRadiusAroundPlayer; }

    protected override void ConstructFSM()
    {
        //Constructing the Enemy States

        CooldownState cooldownState = new CooldownState(2, animator);

        EarthBarrageState earthBarrageState = new EarthBarrageState(this, animator);

        RootSpearsState rootSpearsState = new RootSpearsState(this, animator);

        SpikeShieldState spikeShieldState = new SpikeShieldState();

        RunFromPlayerState runFromPlayerState = new RunFromPlayerState();

        DeadState deadState = new DeadState(animator);



        //Earth Barrage to Wander
        earthBarrageState.AddTransition(TransitionType.Hit, FSMStateType.Cooldown);
        earthBarrageState.AddTransition(TransitionType.AttackOver, FSMStateType.Cooldown);

        //Root Spears to Wander
        rootSpearsState.AddTransition(TransitionType.Hit, FSMStateType.Cooldown);
        rootSpearsState.AddTransition(TransitionType.AttackOver, FSMStateType.Cooldown);

        //Cooldown State
        cooldownState.AddTransition(TransitionType.InAttackRange, FSMStateType.Attacking);
        cooldownState.AddTransition(TransitionType.OutOfRange, FSMStateType.Cooldown);

        //Spike Shield to Running from player
        spikeShieldState.AddTransition(TransitionType.AttackOver, FSMStateType.MovingAway);
        runFromPlayerState.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);
        runFromPlayerState.AddTransition(TransitionType.PlayerInRange, FSMStateType.Attacking);




        //Adding the States
        AddState(cooldownState);
        AddState(earthBarrageState);
        AddState(rootSpearsState);
        AddState(spikeShieldState);
        AddState(runFromPlayerState);
        AddState(deadState);
    }


    #region Earth Barrage Coroutines
    //These scripts are used in the EarthBarrageState. Need to place here since we need Monobehaviour to start Coroutines
    public void SpawnProjectiles(EarthBarrageState barrageState)
    {
        StartCoroutine(SpawnProjectilesCoroutine(barrageState));
    }


    private IEnumerator SpawnProjectilesCoroutine(EarthBarrageState barrageState)
    {
        bool haventSpawnedProjectiles = barrageState.ProjectileGOs.Count == 0; //Object pooling. We can reuse projectiles

        for(int i = 0; i < _barrageProjectileCount; i++)
        {
            if(haventSpawnedProjectiles)
            {
                GameObject go = Instantiate(_barrageProjectilePrefab, transform.position + new Vector3(i - BarrageProjectileCount / 2, 2, 0), Quaternion.identity);
                barrageState.ProjectileGOs.Add(go);
            }
            else
            {
                barrageState.ProjectileGOs[i].SetActive(true);
                barrageState.ProjectileGOs[i].transform.position = transform.position + new Vector3(i - BarrageProjectileCount / 2, 2, 0);
            }
            

            yield return new WaitForSeconds(_barrageProjectileSpawnDelay);
        }

        yield return new WaitForSeconds(_barrageProjectileWaitTimeAfterSpawn);
        barrageState.ProjectileState = EarthBarrageProjectileState.LAUNCH;
    }

    public void LaunchProjectiles(EarthBarrageState barrageState)
    {
        
        StartCoroutine(LaunchProjectilesCoroutine(barrageState));
    }

    private IEnumerator LaunchProjectilesCoroutine(EarthBarrageState barrageState)
    {
        //Launch projectiles one after another towards the player's previous position
        for(int i = 0; i < _barrageProjectileCount; i++)
        {
            barrageState.ProjectileGOs[i].GetComponent<EarthBarrageProjectile>().Launch(barrageState, BarrageProjectileSpeed, player.position);
            yield return new WaitForSeconds(_barrageProjectileLaunchDelay);
        }

        yield return null;
    }

    #endregion

    #region Root Spear Coroutine

    public void SpawnRootSpears(RootSpearsState rootSpearsState)
    {
        StartCoroutine(SpawnProjectilesCoroutine(rootSpearsState));
    }


    private IEnumerator SpawnProjectilesCoroutine(RootSpearsState rootSpearsState)
    {
        bool haventSpawnedProjectiles = rootSpearsState.ProjectileGOs.Count == 0; //Object pooling. We can reuse projectiles
        NavMeshHit samplePoint;

        for (int i = 0; i < _barrageProjectileCount; i++)
        {
            Vector3 spawnLocation = player.position + Random.insideUnitSphere * _rootRadiusAroundPlayer;
            
            NavMesh.SamplePosition(player.position, out samplePoint,50,0);

            spawnLocation = new Vector3(spawnLocation.x, samplePoint.position.y - 10, spawnLocation.z);
            if (haventSpawnedProjectiles)
            {
                GameObject go = Instantiate(_rootSpearPrefab, spawnLocation, Quaternion.identity);
                rootSpearsState.ProjectileGOs.Add(go);
            }
            else
            {
                rootSpearsState.ProjectileGOs[i].SetActive(true);
                rootSpearsState.ProjectileGOs[i].transform.position = spawnLocation;
            }


            yield return new WaitForSeconds(_rootSpearSpawnDelay);
        }

    }

    #endregion

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR

        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 14;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;

        // Enter Range
        Gizmos.color = Color.green;
        textStyle.normal.textColor = Color.green;
        Gizmos.DrawWireSphere(transform.position, ENTER_RANGE);

        Handles.Label(transform.position + transform.forward * ENTER_RANGE, "ENTER RANGE Radius", textStyle);


        // Exit Range
        Gizmos.color = Color.red;
        textStyle.normal.textColor = Color.red;
        Gizmos.DrawWireSphere(transform.position, EXIT_RANGE);

        Handles.Label(transform.position + transform.forward * EXIT_RANGE, "EXIT RANGE Radius", textStyle);



        // Wander Radius
        Handles.color = Color.white;
        textStyle.normal.textColor = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, _wanderRadius, 3);

        Handles.Label(transform.position + transform.forward * _wanderRadius, "Wander Radius", textStyle);



        // Far Range Attack
        Handles.color = Color.magenta;
        textStyle.normal.textColor = Color.magenta;
        Handles.DrawWireDisc(transform.position, Vector3.up, _farRangeFieldRadius, 3);

        Handles.Label(transform.position + transform.forward * _farRangeFieldRadius, "Far Range Radius", textStyle);



        // Mid Range Attack
        Handles.color = Color.yellow;
        textStyle.normal.textColor = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, _midRangeFieldRadius, 3);

        Handles.Label(transform.position + transform.forward * _midRangeFieldRadius, "Mid Range Radius", textStyle);



        // Shield Attack
        Handles.color = Color.cyan;
        textStyle.normal.textColor = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, _spikeShieldRadius, 3);

        Handles.Label(transform.position  + transform.forward * _spikeShieldRadius, "Spike Shield Radius", textStyle);

        #endif
    }
}
