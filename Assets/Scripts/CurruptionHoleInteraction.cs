﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurruptionHoleInteraction : MonoBehaviour
{
    public bool die;

    public GameObject human;
    public GameObject dog;
    public GameObject orb;
    public GameObject pit;

    public Transform point;
    public Transform desiredCameraTransform;

    private Delegate tempDelegate;
    private List<Delegate> delegateList;
    private delegate void Delegate();

    private bool running;
    private float transitionSpeed;
    
    void Start()
    {
        delegateList = new List<Delegate>();

        transitionSpeed = .8f;

    }

    private void Update()
    {

        if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);

        if (running)
        {
            RunToPoint();
            //MoveCamera();
        }
        if (die)
        {            
            foreach (Transform cuppution in transform)
            {
                cuppution.gameObject.GetComponent<Animator>().SetBool("Dead", true);
            }
        } 
    }

    private void LateUpdate()
    {
        if (running)
            MoveCamera();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Human")
        {
            human.gameObject.GetComponent<CharacterController>().enabled = false;
            //human.GetComponent<Animator>().SetTrigger("GiveOrb");
            
            tempDelegate = new Delegate(RunToPoint);
            delegateList.Add(tempDelegate);
            tempDelegate = new Delegate(TurnTowardsPit);
            delegateList.Add(tempDelegate);
            tempDelegate = new Delegate(CallOrb);
            delegateList.Add(tempDelegate);
        }
    }

    private void RunToPoint()
    {
        if (Vector3.Distance(human.transform.position, point.position) > .4f)
        {
            human.transform.position = Vector3.MoveTowards(human.transform.position, point.position, 3.5f * Time.deltaTime);
            human.transform.LookAt(point.transform);
            human.gameObject.GetComponent<HumanMovement>().enabled = false;
            human.gameObject.GetComponent<Animator>().SetBool("Running", true);

            MoveCamera();
        }
        else
            delegateList.RemoveAt(0);
    }

    private void MoveCamera()
    {
        Camera.main.gameObject.GetComponent<ThirdPersonView>().enabled = false;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredCameraTransform.position, transitionSpeed * Time.deltaTime);

        Vector3 currentAngle = new Vector3(
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, desiredCameraTransform.rotation.eulerAngles.x, transitionSpeed * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, desiredCameraTransform.rotation.eulerAngles.y, transitionSpeed * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, desiredCameraTransform.rotation.eulerAngles.z, transitionSpeed * Time.deltaTime));

        Camera.main.transform.eulerAngles = currentAngle;

    }

    private void TurnTowardsPit()
    {
        Vector3 tempVector = pit.transform.position - human.transform.position;
        tempVector.Normalize();
        Vector3 desiredRotation = new Vector3(tempVector.x, 0, tempVector.z);
        human.gameObject.GetComponent<Animator>().SetBool("Running", false);
        if (Vector3.Angle(human.transform.forward, desiredRotation) > 2)
        {
            Quaternion lookRotation = Quaternion.LookRotation(desiredRotation);
            human.transform.rotation = Quaternion.Lerp(human.transform.rotation, lookRotation, 2.5f * Time.deltaTime);
        }
        else
            delegateList.RemoveAt(0);
    }

    private void CallOrb()
    {
        human.gameObject.GetComponent<Animator>().SetTrigger("GiveOrb");
        delegateList.RemoveAt(0);
    }
}
