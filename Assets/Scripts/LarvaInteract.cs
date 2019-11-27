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

    private Vector3 mOffset;
    private Vector3 stickStartPos;
    private Vector3 stickMoveVector;
    private bool stickAtLarva;
    private float mYCoord, mZCoord;
    private float mouseStickSensitiveity;

    private void Start()
    {
        HasCrossed = false;
        mouseStickSensitiveity = 0.1f;
        stickMoveVector = (endPivot.position - startPivot.position).normalized;
        stickStartPos = startPivot.position + stickMoveVector *0.8f;
        pivot.position = stickStartPos;
    }

    private void Update()
    {
        StickMove();
        //LarvaMovement();
    }

    //private void OnMouseDown()
    //{
    //    mYCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).y;
    //    mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

    //    mOffset = gameObject.transform.position - GetMouseWorldPos();
    //}

    //private void OnMouseDrag()
    //{
    //    transform.position = GetMouseWorldPos() + mOffset;
    //}

    //private Vector3 GetMouseWorldPos()
    //{
    //    Vector3 mousePoint = Input.mousePosition;

    //    mousePoint.y = mYCoord;
    //    mousePoint.z = mZCoord;

    //    return Camera.main.ScreenToWorldPoint(mousePoint);
    //}



    //private void LarvaMovement()
    //{
    //    if (!HasCrossed)
    //    {
    //        if (transform.position.x >= stickStart.x)
    //            larva.position = Vector3.MoveTowards(larva.position, new Vector3(pivot.position.x, pivot.position.y, pivot.position.z), .5f * Time.deltaTime);
    //        if (pivot.position == larva.position)
    //            stickAtLarva = true;
    //        if (stickAtLarva && !(transform.position.x <= stickEnd.x))
    //            larva.position = pivot.position;

    //        if (transform.position.x <= stickEnd.x && stickAtLarva)
    //            larva.position = Vector3.MoveTowards(larva.position, new Vector3(endPivot.position.x, endPivot.position.y, endPivot.position.z), .5f * Time.deltaTime);
    //        if (larva.position == endPivot.position)
    //        {
    //            stickAtLarva = false;
    //            HasCrossed = true;
    //        }
    //    }
    //}
    private void StickMove()
    {

        if(Input.GetAxis("Mouse X") <=0)
        {
            pivot.position = Vector3.MoveTowards(pivot.position, endPivot.position, Time.deltaTime);
        }
        if (Input.GetAxis("Mouse X") >= 0)
        {
            pivot.position = Vector3.MoveTowards(pivot.position, startPivot.position, Time.deltaTime);
        }

        Debug.Log("stick: " +pivot.position);
        Debug.Log("start: " + startPivot.position);
        Debug.Log("end: " + endPivot.position);

    }
}
