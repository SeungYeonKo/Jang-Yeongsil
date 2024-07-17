using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCecker : MonoBehaviour
{
    [Header("Boxcast Property")]
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo;

    public bool isGrounded;

    private void Update()
    {
        isGrounded = IsGrounded();
       
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    public bool IsGrounded()
    {
        bool grounded = Physics.BoxCast(transform.position, boxSize, -transform.up, transform.rotation, maxDistance, groundLayer);
        
        return grounded;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }
}
