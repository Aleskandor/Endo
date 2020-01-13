using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCatToGoal : MonoBehaviour
{
    public Transform goal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LarvaInteract.HasCrossed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, goal.rotation, Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, goal.position,0.2f*Time.deltaTime);
        }
    }
}
