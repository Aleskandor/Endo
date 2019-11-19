using UnityEngine;
using System;
using System.Collections.Generic;

public class DogMovement : MonoBehaviour
{
    [Range(0, 1)]
    private Delegate tempDelegate;
    private RaycastHit hit;
    private List<Delegate> delegateList;
    private delegate void Delegate();

    private Transform camTransform;
    private CharacterController charController;
    private Vector3 direction;
    private Vector3 velocity;
    private Vector3 tempPosition;
    private Animator animator;
    private GameObject teleporterPad;
    private CapsuleCollider capsuleCollider;

    private float currentWalkSpeed;
    private float originalWalkSpeed;
    private float currentSpeed;

    private float gravity;

    private float turnSmoothTime;
    private float turnSmoothVelocity;
    private float speedSmoothVelocity;
    private float speedSmoothTime;

    private float velocityY;
    private float pushSpeed;
    private bool canTeleport;

    public bool Locked;

    private void Start()
    {
        animator = GetComponent<Animator>();
        hit = new RaycastHit();
        delegateList = new List<Delegate>();
        camTransform = Camera.main.transform;
        charController = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        currentWalkSpeed = 10;
        originalWalkSpeed = 10;
        pushSpeed = originalWalkSpeed / 2;

        gravity = -12;

        turnSmoothTime = 0.2f;
        speedSmoothTime = 0.1f;

        canTeleport = false;

        Locked = false;
    }

    private void Update()
    {
        if (!Locked)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;

            CheckforDrop();

            if (delegateList.Count != 0)
                delegateList[0].Method.Invoke(this, null);
            else if (Input.GetKeyDown(KeyCode.F))
                CheckForPush();
            else if (Input.GetKeyDown(KeyCode.B) && canTeleport)
                TryToTeleport();
            else
                Move(inputDir);
        }
        else
            Locked = false;
    }

    private void LateUpdate()
    {
        transform.position = tempPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Teleporter")
        {
            teleporterPad = other.gameObject;
            canTeleport = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Teleporter")
        {
            canTeleport = false;
        }
    }

    private void TryToTeleport()
    {
        Locked = true;

        GameObject parent = teleporterPad.transform.parent.gameObject;
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>();
        GameObject otherTeleporterPad = teleporterPad; // Sätter det till "teleporterPad" för att den inte får vara null samt om det sker ett misstag så händer inget

        for (int i = 1; i < childTransforms.Length; i++)
        {
            if (childTransforms[i] != teleporterPad.transform)
                otherTeleporterPad = childTransforms[i].gameObject;
        }

        tempPosition = otherTeleporterPad.transform.position + Vector3.up;
    }

    private void Move(Vector2 inputDir)
    {
        animator.SetBool("Running", false);

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            animator.SetBool("Running", true);
        }

        float targetSpeed = currentWalkSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocityY += Time.deltaTime * gravity;
        velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        charController.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(charController.velocity.x, charController.velocity.z).magnitude;

        if (charController.isGrounded)
        {
            velocityY = 0;
        }
    }

    private void CheckforDrop()
    {
        if (delegateList.Count == 0)
        {
            RaycastHit groundHit;

            int lookDist = 100;

            float minDistToGround = 4;
            float maxDistToGround = 7;

            if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2) + (transform.forward * 1.1f), Vector3.down, out groundHit, lookDist))
            {
                if (minDistToGround < groundHit.distance && groundHit.distance < maxDistToGround)
                    currentWalkSpeed = 0;               
                else if (minDistToGround < groundHit.distance && groundHit.distance > maxDistToGround)
                    currentWalkSpeed = 0;
                else
                    currentWalkSpeed = originalWalkSpeed;
            }
        }
    }

    private void CheckForPush()
    {
        if (delegateList.Count == 0)
        {
            RaycastHit straightHit;

            int lookDist = 10;

            // The acceptable distance we can be away from the wall in our forward plane when we raycast to detect a wall
            float acceptableDist = 4f;

            if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2), transform.forward, out straightHit, lookDist))
            {
                if (straightHit.collider.tag == "Pushable")
                {
                    direction = -straightHit.normal;

                    if (straightHit.distance < acceptableDist)
                    {
                        hit = straightHit;

                        // Add the order of events that will comprise this action
                        tempDelegate = new Delegate(TurnTowardsWall);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(MoveAwayFromWall);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(TriggerDogPushingTransitionAnimation);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(Push);
                        delegateList.Add(tempDelegate);
                    }
                }
            }
        }
    }

    private void TurnTowardsWall()
    {
        if (Vector3.Angle(transform.forward, -hit.normal) > 2)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-hit.normal);

            // Rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }
        else
        {
            delegateList.RemoveAt(0);
        }
    }

    private void MoveAwayFromWall()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);

        if (hit.distance < 2)
        {
            tempPosition += -transform.forward * Time.deltaTime;
        }
        else
            delegateList.RemoveAt(0);
    }

    private void Push()
    {
        if (Input.GetKey(KeyCode.F))
        {
            animator.SetBool("Pushing", true);
            velocity = direction * pushSpeed;
            charController.Move(velocity * Time.deltaTime);
            hit.collider.gameObject.GetComponent<PushObject>().Move(velocity);
        }
        else
        {
            animator.SetBool("Pushing", false);
            delegateList.RemoveAt(0);
        }
    }

    private void RemoveDelegate()
    {
        delegateList.RemoveAt(0);
        animator.SetBool("PushTransition", false);
    }

    private void TriggerDogPushingTransitionAnimation()
    {
        if (!animator.GetBool("PushTransition"))
            animator.SetBool("PushTransition", true);
    }
}