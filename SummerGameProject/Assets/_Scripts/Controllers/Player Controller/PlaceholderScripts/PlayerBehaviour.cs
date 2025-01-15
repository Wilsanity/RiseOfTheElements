using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;

    public float jumpVelocity = 7.5f;

    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;

    private float vInput;
    private float hInput;

    private Rigidbody _rb;

    private CapsuleCollider _col;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
    }
    void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;
    }

    void FixedUpdate()
    {
        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);
        _rb.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * angleRot);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(_col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);
        return grounded;
    }

    //public void Heal(float healValue)
    //{
    //    FindObjectOfType<GameBehaviour>().SetHP(healValue);
    //}


   
}
