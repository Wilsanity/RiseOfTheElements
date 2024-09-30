using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    protected Vector3 destPos; // next destination position
    protected float elapsedTime;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

    // Use this for intialization
    void Start() { Initialize(); }

    void Update() { FSMUpdate(); }

    void FixedUpdate() { FSMFixedUpdate(); }
}
