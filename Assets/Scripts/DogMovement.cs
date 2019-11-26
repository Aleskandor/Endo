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

    private CharacterController charController;
    private Transform camTransform;
    private Vector3 direction;
    private Vector3 velocity;
    private Animator animator;
    private GameObject teleporterPad;
    private GameObject otherTeleporterPad;
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

        currentWalkSpeed = 7.5f;
        originalWalkSpeed = 7.5f;
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

            CheckForLedges();

            if (delegateList.Count != 0)
                delegateList[0].Method.Invoke(this, null);
            else if (Input.GetKeyDown(KeyCode.F))
                CheckForPush();
            else if (Input.GetKeyDown(KeyCode.B) && canTeleport)
                CheckForTeleport();
            else
                Move(inputDir);
        }
        else
            Locked = false;
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

    private void CheckForLedges()
    {
        RaycastHit groundHit;

        int lookDist = 100;
        float minDistToGround = 4;

        if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2) + (transform.forward * 1.1f), Vector3.down, out groundHit, lookDist))
        {
            if (groundHit.distance > minDistToGround)
                currentWalkSpeed = 0;
            else
                currentWalkSpeed = originalWalkSpeed;
        }
    }

    private void CheckForPush()
    {
        RaycastHit straightHit;

        int lookDist = 10;
        float acceptableDist = 4f;

        if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2), transform.forward, out straightHit, lookDist))
        {
            if (straightHit.collider.tag == "Pushable")
            {
                direction = -straightHit.normal;

                if (straightHit.distance < acceptableDist)
                {
                    hit = straightHit;

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

    private void CheckForTeleport()
    {
        GameObject parent = teleporterPad.transform.parent.gameObject;
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>();
        otherTeleporterPad = teleporterPad;

        for (int i = 1; i < childTransforms.Length; i++)
        {
            if (childTransforms[i] != teleporterPad.transform)
                otherTeleporterPad = childTransforms[i].gameObject;
        }

        tempDelegate = new Delegate(TurnTowardsTeleport);
        delegateList.Add(tempDelegate);
        tempDelegate = new Delegate(MoveAwayFromTeleport);
        delegateList.Add(tempDelegate);
        tempDelegate = new Delegate(TriggerCrouchInAnimation);
        delegateList.Add(tempDelegate);
        tempDelegate = new Delegate(TriggerCrouchTransitionAnimation);
        delegateList.Add(tempDelegate);
    }

    private void TurnTowardsTeleport()
    {
        if (Vector3.Angle(transform.forward, -teleporterPad.transform.up) > 2)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-teleporterPad.transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime);
        }
        else
            delegateList.RemoveAt(0);
    }

    private void MoveAwayFromTeleport()
    {
        Vector2 dogPosV2 = new Vector2(transform.position.x, transform.position.z);
        Vector2 teleportPosV2 = new Vector2(teleporterPad.transform.position.x, teleporterPad.transform.position.z);

        if (Vector2.Distance(dogPosV2, teleportPosV2) < 2.25)
            charController.Move(-transform.forward * Time.deltaTime);
        else
            delegateList.RemoveAt(0);
    }

    private void Teleport()
    {
        Vector3 dogPositionWithTeleporterHeight = new Vector3(transform.position.x, teleporterPad.transform.position.y, transform.position.z);
        Vector3 distance = (otherTeleporterPad.transform.position - dogPositionWithTeleporterHeight) + otherTeleporterPad.transform.up * (capsuleCollider.height / 2);

        transform.position += distance;
        transform.Rotate(transform.rotation.x, otherTeleporterPad.transform.rotation.y + 180, transform.rotation.z);
        animator.SetTrigger("CrouchTransition");
    }

    private void TurnTowardsWall()
    {
        if (Vector3.Angle(transform.forward, -hit.normal) > 2)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-hit.normal);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }
        else
            delegateList.RemoveAt(0);
    }

    private void MoveAwayFromWall()
    {
        RaycastHit tempHit;
        Physics.Raycast(transform.position + (Vector3.up * charController.height / 2), transform.forward, out tempHit);

        if (tempHit.distance < 1.75)
            charController.Move(-transform.forward * Time.deltaTime);
        else
            delegateList.RemoveAt(0);
    }

    private void Push()
    {
        PushObject boxPO = hit.collider.gameObject.GetComponent<PushObject>();

        if (Input.GetKey(KeyCode.F) && boxPO.CheckForLedges(transform.forward))
        {
            animator.SetBool("Pushing", true);
            velocity = direction * pushSpeed;
            charController.Move(velocity * Time.deltaTime);
            boxPO.Move(velocity);
        }
        else
        {
            animator.SetBool("Pushing", false);
            animator.SetBool("PushTransition", false);
            delegateList.RemoveAt(0);
        }
    }

    private void TriggerDogPushingTransitionAnimation()
    {
        if (!animator.GetBool("PushTransition"))
        {
            animator.SetBool("PushTransition", true);
            animator.SetBool("Pushing", true);
        }
    }

    private void TriggerCrouchInAnimation()
    {
        if (!animator.GetBool("Crouching"))
            animator.SetBool("Crouching", true);
    }

    private void TriggerCrouchTransitionAnimation()
    {
        animator.SetTrigger("CrouchTransition");
    }

    private void RemoveDelegate()
    {
        delegateList.RemoveAt(0);
        animator.SetBool("PushTransition", false);
        animator.SetBool("Crouching", false);
    }
}