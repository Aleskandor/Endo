using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Teleport : MonoBehaviour
{
    private float disabletimer = 2;
    private bool available = false;

    public int code;

    private HumanMovement cMove;
    private Collider otherCollider;

    private void Update()
    {
        if (disabletimer > 0)
        {
            disabletimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (available)
        {
            if (Input.GetKey(KeyCode.B) && disabletimer <= 0)
            {
                disabletimer = 2;
                cMove = otherCollider.gameObject.GetComponent<HumanMovement>();
                cMove.Locked = true;

                foreach (Teleport tp in FindObjectsOfType<Teleport>())
                {
                    if (tp.code == code && tp != this)
                    {
                        tp.disabletimer = 2;
                        Vector3 position = tp.gameObject.transform.position;
                        position.y += 1;
                        otherCollider.gameObject.transform.position = position;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Human" || other.name == "Dog")
        {
            otherCollider = other;
            available = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Human" || other.name == "Dog")
        {
            available = false;
        }
    }
}