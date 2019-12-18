using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PushObject : MonoBehaviour
{
    private CharacterController charController;
    private NavMeshSurface navMeshSurface;
    private BoxCollider boxCollider;
    private Vector3 playerDirection;
    private GameObject childCube;

    private float gravity;

    public bool beingPushed;
    public bool inHole;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        charController = GetComponent<CharacterController>();
        navMeshSurface = GameObject.FindGameObjectWithTag("Environment").GetComponent<NavMeshSurface>();
        childCube = transform.GetChild(0).gameObject;

        gravity = -9.8f;

        inHole = false;
        beingPushed = false;
    }

    private void Update()
    {
        if (!charController.isGrounded && !beingPushed)
            charController.Move(new Vector3(0, gravity * Time.deltaTime, 0));

        if (charController.isGrounded && inHole && !gameObject.isStatic)
        {
            AudioSource ac = GetComponents<AudioSource>()[1];
            ac.Play();
            gameObject.isStatic = true;
            childCube.isStatic = true;
            gameObject.layer = 0;
            childCube.layer = 0;
            navMeshSurface.BuildNavMesh();
        }
    }

    public bool CheckForObstacle(Vector3 dir)
    {
        float offSetFromGround = 0.25f;
        RaycastHit hit;
        Vector3 rayOri = new Vector3(transform.position.x, transform.position.y + offSetFromGround, transform.position.z) + dir * 1.9f;

        if (Physics.Raycast(transform.position + transform.up*0.1f, Vector3.down, 0.2f))
        {
            if (Physics.Raycast(rayOri, dir, out hit, Mathf.Infinity))
            {
                if (hit.distance < 4)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else
        {
            charController.Move(Vector3.zero);
            return true;
        }
    }
}