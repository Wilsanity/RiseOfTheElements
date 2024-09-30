using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmosSphere : MonoBehaviour
{
    [SerializeField] float radius = 0.3f;
    [SerializeField] Color gizmosColour = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColour;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
