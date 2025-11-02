using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turningRate = 270f;

    private Vector2 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.MoveEvent += HandleMovement;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.MoveEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }
    void Update()
    {
        if (!IsOwner) return;
        float zRotation = -previousMovementInput.x * turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    void FixedUpdate()
    {
        if (!IsOwner) return; 
        rb.linearVelocity = (Vector2)bodyTransform.up * previousMovementInput.y * moveSpeed;
    }
}
