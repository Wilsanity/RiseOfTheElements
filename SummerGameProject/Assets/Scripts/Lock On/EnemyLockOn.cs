using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyLockOn : MonoBehaviour
{
    Transform currentTarget;
    PlayerInput playerInput;

    #region input actions

    InputAction lockOnAction;

    #endregion

    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform enemyTarget_Locator;

    [Tooltip("StateDrivenMethod for Switching Cameras")]
    [SerializeField] Animator cinemachineAnimator;

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look;
    [SerializeField] float noticeZone = 10;
    [SerializeField] float lookAtSmoothing = 2;
    [Tooltip("Angle_Degree")] [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float crossHair_Scale = 0.1f;

    
    Transform cam;
    bool enemyLocked;
    float currentYOffset;
    Vector3 pos;

    //[SerializeField] CameraFollow camFollow;
    [SerializeField] Transform lockOnCanvas;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lockOnAction = playerInput.actions["Lock In"];

        lockOnAction.started += ctx => LockIn();
    }

    void Start()
    {
        cam = Camera.main.transform;
        lockOnCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (enemyLocked) 
        {
            if (!TargetInRange())
            {
                ResetTarget();
            }

            LookAtTarget();
        }

    }

    void LockIn()
    {
        if (currentTarget)
        {
            //If there is already a target, Reset.
            ResetTarget();
            return;
        }

        if (currentTarget = ScanNearBy())
        {
            FoundTarget();
        }
        else
        {
            ResetTarget();
        }
    }


    void FoundTarget()
    {
        lockOnCanvas.gameObject.SetActive(true);
        cinemachineAnimator.Play("TargetCamera");
        enemyLocked = true;
    }

    void ResetTarget()
    {
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        enemyLocked = false;
        cinemachineAnimator.Play("FollowCamera");
    }


    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;

        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float angle = Vector3.Angle(cam.forward, dir);
            
            if (angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = angle;      
            }
        }

        if (!closestTarget ) return null;
        
        Vector3 tarPos = closestTarget.GetComponentInChildren<Transform>(gameObject.name == "Lock On Target").position;

        if (Blocked(tarPos)) return null;

        return closestTarget;
    }

    bool Blocked(Vector3 t)
    {
        RaycastHit hit;

        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
        {
            if (!hit.transform.CompareTag("Enemy")) return true;
        }

        return false;
    }

    bool TargetInRange()
    {
        float dis = (transform.position - pos).magnitude;
        if (dis/2 > noticeZone) return false; else return true;
    }


    private void LookAtTarget()
    {
        if (currentTarget == null) 
        {
            ResetTarget();
            return;
        }

        pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
        lockOnCanvas.position = pos;
        lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);

        enemyTarget_Locator.position = pos;
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, noticeZone);   
    }
}
