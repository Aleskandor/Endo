using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessForest : MonoBehaviour
{
    private Vector3 tempPosition;
    private BoxCollider boxCollider;
    private int count;
    private DialogueTrigger dt;


    public GameObject player;

    private void Start()
    {
        tempPosition = new Vector3();

        boxCollider = GetComponent<BoxCollider>();
        dt = GetComponent<DialogueTrigger>();
    }

    private void Update()
    {
        Vector3 playerPosition = player.transform.position;
        tempPosition = Vector3.zero;

        if (playerPosition.x <= transform.position.x - boxCollider.size.x / 2)
        {
            tempPosition += new Vector3(boxCollider.size.x, 0, 0);
            count += 1;
        }
        else if (playerPosition.x >= transform.position.x + boxCollider.size.x / 2)
        {
            tempPosition += new Vector3(-boxCollider.size.x, 0, 0);
            count += 1;
        }
 

        if (playerPosition.z <= transform.position.z - boxCollider.size.z / 2)
        {
            tempPosition += new Vector3(0, 0, boxCollider.size.z);
            count += 1;
        }
        else if (playerPosition.z >= transform.position.z + boxCollider.size.z / 2)
        {
            tempPosition += new Vector3(0, 0, -boxCollider.size.z);
            count += 1;
        }

        if (tempPosition != Vector3.zero)
        {
            HumanMovement cMove = player.GetComponent<HumanMovement>();
            cMove.Locked = true;
            player.transform.position += tempPosition;
        }

        if (count > 4)
        {
            dt.TriggerDialogue();
            count = 0;
        }

        tempPosition = Vector3.zero;
    }
}
