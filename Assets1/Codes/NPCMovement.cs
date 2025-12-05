using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; // drag & drop dito sa Inspector

    [Header("Movement Settings")]
    public float moveSpeed = 2f;  // walking speed
    public float stopDistance = 1.5f; // distance bago huminto

    [Header("Gravity Settings")]
    public float gravity = -9.81f; // customizable sa Inspector

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (target == null) return;

        // Distance sa target
        float distance = Vector3.Distance(transform.position, target.position);

        // Sundan target kung malayo pa
        if (distance > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // stay sa ground
            controller.Move(direction * moveSpeed * Time.deltaTime);

            // Rotate NPC papunta sa target
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f; // keep grounded

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
