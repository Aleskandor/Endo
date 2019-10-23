using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessForest : MonoBehaviour
{
    private Vector3 tempPosition;
    private BoxCollider boxCollider;

    public GameObject player;

    private void Start()
    {
        tempPosition = new Vector3();

        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Vector3 playerPosition = player.transform.position;
        tempPosition = Vector3.zero;

        if (playerPosition.x <= transform.position.x - boxCollider.size.x / 2)
            tempPosition += new Vector3(boxCollider.size.x, 0, 0);
        else if (playerPosition.x >= transform.position.x + boxCollider.size.x / 2)
            tempPosition += new Vector3(-boxCollider.size.x, 0, 0);

        if (playerPosition.z <= transform.position.z - boxCollider.size.z / 2)
            tempPosition += new Vector3(0, 0, boxCollider.size.z);
        else if (playerPosition.z >= transform.position.z + boxCollider.size.z / 2)
            tempPosition += new Vector3(0, 0, -boxCollider.size.z);

        if (tempPosition != Vector3.zero)
        {
            CharacterMovement cMove = player.GetComponent<CharacterMovement>();
            cMove.Locked = true;
            player.transform.position += tempPosition;
        }

        tempPosition = Vector3.zero;
    }
}
