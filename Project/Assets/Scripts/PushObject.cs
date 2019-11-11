using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObject : MonoBehaviour
{
    public float gravity;

    private CharacterController controller;

    private void Start()
    {
        gravity = -9.8f;

        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!controller.isGrounded)
        {
            controller.Move(new Vector3(0, gravity * Time.deltaTime, 0));
        }            
    }

    public void Move(Vector3 velocity)
    {
        controller.Move(velocity * Time.deltaTime);
    }
}