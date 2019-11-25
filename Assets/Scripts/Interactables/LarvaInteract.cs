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

    private Vector3 mOffset;
    private Vector3 stickStart;
    private Vector3 stickEnd;
    private bool stickAtLarva;
    private float mYCoord, mZCoord;


    private void Start()
    {
        HasCrossed = false;

        stickStart = new Vector3(-71.9f, 3f, 249.6f);
        stickEnd = new Vector3(-72.94f, 2.89f, 250.47f);
    }

    private void Update()
    {
        Constraints();
        LarvaMovement();
        
    }
    private void OnMouseDown()
    {
        mYCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).y;
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.y = mYCoord;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void Constraints()
    {
        //Constraints for the Larva tree
        if (transform.position.x > stickStart.x)
            transform.position = new Vector3(stickStart.x, transform.position.y, transform.position.z);
        if (transform.position.y > stickStart.y)
            transform.position = new Vector3(transform.position.x, stickStart.y, transform.position.z);
        if (transform.position.z < stickStart.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, stickStart.z);

        //Constraints for the other tree
        if (transform.position.x < stickEnd.x)
            transform.position = new Vector3(stickEnd.x, transform.position.y, transform.position.z);
        if (transform.position.y < stickEnd.y)
            transform.position = new Vector3(transform.position.x, stickEnd.y, transform.position.z);
        if (transform.position.z > stickEnd.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, stickEnd.z);
    }

    private void LarvaMovement()
    {
        if (!HasCrossed)
        {
            if (transform.position.x >= stickStart.x)
                larva.position = Vector3.MoveTowards(larva.position, new Vector3(pivot.position.x, pivot.position.y, pivot.position.z), .5f * Time.deltaTime);
            if (pivot.position == larva.position)
                stickAtLarva = true;
            if (stickAtLarva && !(transform.position.x <= stickEnd.x))
                larva.position = pivot.position;

            if (transform.position.x <= stickEnd.x && stickAtLarva)
                larva.position = Vector3.MoveTowards(larva.position, new Vector3(endPivot.position.x, endPivot.position.y, endPivot.position.z), .5f * Time.deltaTime);
            if (larva.position == endPivot.position)
            {
                stickAtLarva = false;
                HasCrossed = true;
            }
        }
    }
}
