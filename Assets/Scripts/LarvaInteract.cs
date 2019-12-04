using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LarvaInteract : MonoBehaviour
{
    public static bool HasCrossed;
    public Transform larva;
    public Transform pivot;
    public Transform endPivot;
    public Transform startPivot;
    public Animator larvaAni;
    private bool puzzleCompleted = false;
    private bool hasPlayedSound = false;

    private Vector3 stickStartPos;
    private Vector3 stickMoveVector;
    private bool stickAtLarva, stickAtStart, stickAtEnd;


    private void Start()
    {
        HasCrossed = false;
        stickMoveVector = (endPivot.position - startPivot.position).normalized;
        stickStartPos = startPivot.position + stickMoveVector * 0.8f;
        pivot.position = stickStartPos;
    }

    private void Update()
    {
        StickMove();
        LarvaMovement();
        Conditions();
    }
    private void LarvaMovement()
    {
        if (stickAtStart)
        {
            larvaAni.SetBool("Moving", true);
            larva.position = Vector3.MoveTowards(larva.position, pivot.position, Time.deltaTime);
        }

        if (stickAtLarva)
        {
            larvaAni.SetBool("Moving", false);
            larva.position = pivot.position;
        }

        if (stickAtEnd)
        {
            larvaAni.SetBool("Moving", true);
            larva.position = Vector3.MoveTowards(larva.position, endPivot.position, Time.deltaTime);
        }
    }
    private void StickMove()
    {
        if (!stickAtEnd || !stickAtStart)
        {
            if (Input.GetAxis("Mouse X") <= 0)
            {
                pivot.position = Vector3.MoveTowards(pivot.position, endPivot.position, Time.deltaTime);
            }
            if (Input.GetAxis("Mouse X") >= 0)
            {
                pivot.position = Vector3.MoveTowards(pivot.position, startPivot.position, Time.deltaTime);
            }
        }
    }
    private void Conditions()
    {
        if (pivot.position == startPivot.position && !stickAtLarva && !HasCrossed)
            stickAtStart = true;
        else
            stickAtStart = false;

        if (pivot.position == endPivot.position && stickAtLarva)
        {
            if (!hasPlayedSound)
            {
                SoundManager.instance.Play("Victory");
                hasPlayedSound = true;
            }

            stickAtEnd = true;
            HasCrossed = true;
        }
        else
            stickAtEnd = false;

        if (larva.position == pivot.position)
            stickAtLarva = true;
        else
            stickAtLarva = false;
    }
}
