using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObject : MonoBehaviour
{
    private float distance;
    private float friction;

    private int range;

    public float gravity;

    private Transform currentTransform;
    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 direction;

    public ThirdPersonView TPV;

    private void Start()
    {
        friction = 0.96f;
        range = 5;
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

        if (Input.GetKey(KeyCode.E))
        {
            if (InRange())
            {
                if (CheckSide())
                {
                    Move();
                }
            }
        }
        else
        {
            if (TPV.GetCurrentTarget().name == "Human")
                TPV.GetCurrentTarget().GetComponent<HumanMovement>().pushing = false;
            else
                TPV.GetCurrentTarget().GetComponent<DogMovement>().pushing = false;
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
    }

    private bool CheckSide()
    {
        RaycastHit hit;

        if (Physics.Raycast(currentTransform.position + Vector3.up, currentTransform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.name == "PushableObject")
            {
                direction = -hit.normal;
                if (TPV.GetCurrentTarget().name == "Human")
                {
                    TPV.GetCurrentTarget().GetComponent<HumanMovement>().pushing = true;
                    velocity = direction * TPV.GetCurrentTarget().GetComponent<HumanMovement>().pushSpeed;
                    TPV.GetCurrentTarget().GetComponent<HumanMovement>().velocity = velocity;
                }
                else
                {
                    TPV.GetCurrentTarget().GetComponent<DogMovement>().pushing = true;
                    velocity = direction * TPV.GetCurrentTarget().GetComponent<DogMovement>().pushSpeed;
                    TPV.GetCurrentTarget().GetComponent<DogMovement>().velocity = velocity;
                }

                return true;
            }
        }
        return false;
    }
}