using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObject : MonoBehaviour
{
    private float distance;
    private float friction;

    private int range;

    private bool pushed;

    public float currentSpeed;
    public float gravity;

    private Transform currentTransform;
    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 direction;

    public ThirdPersonView TPV;

    private void Start()
    {
        friction = 0.96f;
        range = 10;
        pushed = false;
        currentSpeed = 5f;
        gravity = -9.8f;

        controller = GetComponent<CharacterController>();
        currentTransform = TPV.target.transform;
    }

    private void Update()
    {
        currentTransform = TPV.target.transform;
        distance = Vector3.Distance(transform.position, currentTransform.position);

        if (!controller.isGrounded)
        {
            controller.Move(new Vector3(0, gravity * Time.deltaTime, 0));
        }

        if (Input.GetKeyDown(KeyCode.E) && InRange())
        {
            if (CheckSide())
            {
                pushed = true;
            }
        }

        if (pushed)
        {
            Move();
        }
    }

    bool InRange()
    {
        if (distance < range)
            return true;
        else
            return false;
    }

    void Move()
    {
        velocity *= friction;

        controller.Move(velocity * Time.deltaTime);

        if (velocity.magnitude < 0.3f)
        {
            pushed = false;
        }
    }

    private bool CheckSide()
    {
        RaycastHit hit;

        if (Physics.Raycast(currentTransform.position + Vector3.up, currentTransform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.name == "PushableObject")
            {
                direction = -hit.normal;
                velocity = direction * currentSpeed;
                return true;
            }
        }
        return false;
    }
}