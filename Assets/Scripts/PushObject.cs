using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PushObject : MonoBehaviour
{
    [Range(0, 1)]
    private Delegate tempDelegate;
    private RaycastHit hit;
    private List<Delegate> delegateList;
    private delegate void Delegate();
    private CharacterController charController;
    private NavMeshSurface navMeshSurface;
    private BoxCollider boxCollider;
    private Vector3 playerDirection;
    private GameObject childCube;

    private bool canMove;
    private float gravity;
    private int lookDist;

    public bool inHole;

    private void Start()
    {
        hit = new RaycastHit();
        delegateList = new List<Delegate>();

        boxCollider = GetComponent<BoxCollider>();
        charController = GetComponent<CharacterController>();
        navMeshSurface = GameObject.FindGameObjectWithTag("Environment").GetComponent<NavMeshSurface>();
        childCube = transform.GetChild(0).gameObject;

        canMove = true;
        gravity = -9.8f;
        lookDist = 100;

        inHole = false;
    }

    private void Update()
    {
        if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);
        else if (!charController.isGrounded)
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

    public void StopPlayingSound()
    {
        if (delegateList.Count == 0)
        {
            if (GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Stop();
            }
        }
    }

    public void Move(Vector3 velocity)
    {
        if (canMove)
            charController.Move(velocity * Time.deltaTime);

        //if (Math.Abs(velocity.x) > 1 || Math.Abs(velocity.z) > 1)
        //{
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
        //}
    }

    public bool CheckForLedges(Vector3 direction)
    {
        if (delegateList.Count == 0)
        {
            playerDirection = direction; //Used in MoveOffLedge, but can't send in as perameter.

            if (Physics.Raycast(transform.position + (Vector3.up * boxCollider.size.y / 2) + direction * boxCollider.size.x / 2, Vector3.down, out hit, lookDist))
            {
                if (hit.distance < boxCollider.size.y / 2 - 0.05)
                {
                    if (GetComponent<AudioSource>().isPlaying)
                    {
                        GetComponent<AudioSource>().Stop();
                    }
                    canMove = false;
                    return false;
                }
                else if (hit.distance > boxCollider.size.y / 2 + 0.1)
                {
                    tempDelegate = new Delegate(MoveOffLedge);
                    delegateList.Add(tempDelegate);

                    canMove = false;
                    return false;
                }
                else
                {
                    canMove = true;
                    return true;
                }
            }
            else
                return false;
        }
        else
            return false;
    }

    public void MoveOffLedge()
    {
        Physics.Raycast(transform.position + (playerDirection * (boxCollider.size.x / 2)) + -transform.up, -playerDirection, out hit, lookDist);

        if (hit.distance < boxCollider.size.x)
            charController.Move(playerDirection * Time.deltaTime * 2);
        else
        {
            if (GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Stop();
            }
            delegateList.RemoveAt(0);
        }
    }
}