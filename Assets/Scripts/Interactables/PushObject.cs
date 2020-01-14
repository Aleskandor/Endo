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
    private bool stop = false;

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

        if (charController.isGrounded && inHole && !gameObject.isStatic && !stop)
        {
            AudioSource ac = GetComponents<AudioSource>()[1];
            ac.Play();
            gameObject.isStatic = true;
            childCube.isStatic = true;
            gameObject.layer = 11;
            childCube.layer = 11;
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            stop = true;
        }
    }

    public bool CheckForObstacle(Vector3 dir)
    {
        float offSetFromGround = 0.15f;
        RaycastHit hit;
        Vector3 rayOri = new Vector3(transform.position.x, transform.position.y + offSetFromGround, transform.position.z) + dir * 1.9f;

        if (Physics.Raycast(transform.position + transform.up*0.1f, Vector3.down, 0.2f))
        {
            if (Physics.Raycast(rayOri, dir, out hit, Mathf.Infinity))
            {
                if (hit.distance < 4)
                    return true;
                else
                {
                    if (!GetComponent<AudioSource>().isPlaying)
                    {
                        GetComponent<AudioSource>().Play();
                    }
                    return false;
                }
            }
            else
            {
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().Play();
                }
                return false;
            }
        }
        else
        {
            charController.Move(Vector3.zero);
            return true;
        }
    }

    public void StopPlayingSound()
    {
        if (GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Stop();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "thudCube")
        {
            AudioSource ac = GetComponents<AudioSource>()[1];
            ac.Play();
        }
    }
}