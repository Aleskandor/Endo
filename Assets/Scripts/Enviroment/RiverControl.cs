using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverControl : MonoBehaviour
{
    Vector3 endPosition;
    Vector3 startPosition;

    public GameObject StartDoor;
    public GameObject EndDoor;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        endPosition = new Vector3(transform.position.x, transform.position.y -3, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (LarvaInteract.HasCrossed)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, 0.5f * Time.deltaTime);
            StartDoor.SetActive(false);
            EndDoor.SetActive(false);
        }
    }
}
