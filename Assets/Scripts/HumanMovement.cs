using UnityEngine;
using System;
using System.Collections.Generic;

public class HumanMovement : MonoBehaviour
{
    [Range(0, 1)]
    private Delegate tempDelegate;
    private RaycastHit hit;
    private List<Delegate> delegateList;
    private delegate void Delegate();

    private CharacterController charController;
    private Transform camTransform;
    private Vector3 velocity;
    private Vector3 direction;
    private Animator animator;
    private GameObject teleporterPad;

    private float distanceToClimb;
    private float distanceToWalk;
    private float distanceWalked;

    private float currentWalkSpeed;
    private float originalWalkSpeed;
    private float pushSpeed;
    private float currentSpeed;

    private float turnSmoothTime;
    private float turnSmoothVelocity;
    private float speedSmoothVelocity;
    private float speedSmoothTime;

    private float velocityY;
    private float gravity;

    public bool Locked;

    private void Start()
    {
        animator = GetComponent<Animator>();
        hit = new RaycastHit();
        delegateList = new List<Delegate>();
        camTransform = Camera.main.transform;
        charController = GetComponent<CharacterController>();

        currentWalkSpeed = 9f;
        originalWalkSpeed = 9f;
        pushSpeed = 3;

        gravity = -12;

        turnSmoothTime = 0.2f;
        speedSmoothTime = 0.1f;

        Locked = false;
    }

    private void Update()
    {
        if (!Locked)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;

            CheckforLedges();

            if (delegateList.Count != 0)
                delegateList[0].Method.Invoke(this, null);
            else if (Input.GetKeyDown(KeyCode.R))
            {
                CheckForClimb();
                CheckforDrop();
            }
            else if (Input.GetKeyDown(KeyCode.F))
                CheckForPush();
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SoundManager.instance.PlayWhistle();
            }
            else
                Move(inputDir);
        }
        else
            Locked = false;
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

    private void CheckForClimb()
    {
        RaycastHit straightHit;
        RaycastHit overLedgeHit;

        int layerMask = 1 << 8;
        int lookDist = 100;
        float acceptableDist = 3f;
        float minClimbHeight = 19f;
        float maxClimbHeight = 18f;

        if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2), transform.forward, out straightHit, lookDist, layerMask))
        {
            if (straightHit.distance < acceptableDist)
            {
                if (Physics.Raycast(transform.position + (transform.forward * 1.5f) + (transform.up * charController.height * 5), -transform.up, out overLedgeHit, lookDist, layerMask))
                {
                    if (overLedgeHit.distance > maxClimbHeight && overLedgeHit.distance < minClimbHeight)
                    {
                        hit = straightHit;
                        distanceToClimb = overLedgeHit.point.y - transform.position.y;
                        distanceToWalk = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(overLedgeHit.point.x, overLedgeHit.point.z)) + charController.radius;

                        tempDelegate = new Delegate(TurnTowardsWall);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(MoveAwayFromWall);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(TriggerClimbUpAnimation);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(ClimbUp);
                        delegateList.Add(tempDelegate);
                    }
                }
            }
        }
    }

    private void CheckforDrop()
    {
        if (delegateList.Count == 0)
        {
            RaycastHit groundHit;

            int layerMask = 1 << 8;
            int lookDist = 100;
            float minDistToGround = 4;
            float maxDistToGround = 7;

            if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2) + (transform.forward * 1.1f), Vector3.down, out groundHit, lookDist))
            {
                if (minDistToGround < groundHit.distance && groundHit.distance < maxDistToGround)
                {
                    distanceToClimb = groundHit.point.y - transform.position.y;

                    if (Physics.Raycast(transform.position + (transform.forward * 1f) + (-transform.up * 1), -transform.forward, out hit, lookDist, layerMask))
                    {
                        float animationOffset = 1.5f;
                        distanceToWalk = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(hit.point.x, hit.point.z)) + charController.radius + animationOffset;

                        tempDelegate = new Delegate(TurnAwayFromWall);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(TriggerClimbDownAnimation);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(ClimbDown);
                        delegateList.Add(tempDelegate);
                    }
                }
            }
        }
    }

    private void CheckforLedges()
    {
        RaycastHit groundHit;

        int lookDist = 100;
        float minDistToGround = 4;

        if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2) + (transform.forward), Vector3.down, out groundHit, lookDist))
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
        float acceptableDist = 2f;

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
                    tempDelegate = new Delegate(Push);
                    delegateList.Add(tempDelegate);
                }
            }
        }
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

    private void TurnAwayFromWall()
    {
        if (Vector3.Angle(transform.forward, hit.normal) > 2)
        {
            Quaternion lookRotation = Quaternion.LookRotation(hit.normal);

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
        Physics.Raycast(transform.position + (Vector3.up * charController.height / 2), transform.forward, out hit);

        if (hit.distance < 1.5)
        {
            charController.Move(-transform.forward * Time.deltaTime);
        }
        else
            delegateList.RemoveAt(0);
    }

    private void ClimbUp()
    {
        animator.SetBool("ClimbUp", false);
        charController.Move(Vector3.up * distanceToClimb + transform.forward * distanceToWalk);
        delegateList.RemoveAt(0);
    }

    private void ClimbDown()
    {
        animator.SetBool("ClimbDown", false);
        charController.Move(Vector3.up * distanceToClimb + transform.forward * distanceToWalk);
        delegateList.RemoveAt(0);
    }

    private void WalkForwards()
    {
        if (distanceWalked < distanceToWalk)
        {
            distanceWalked += transform.forward.magnitude * Time.deltaTime;
            charController.Move(transform.forward * Time.deltaTime);
        }
        else
        {
            distanceWalked = 0;
            delegateList.RemoveAt(0);
        }
    }

    private void Push()
    {
        PushObject boxPO = hit.collider.gameObject.GetComponent<PushObject>();

        if (Input.GetKey(KeyCode.F) && boxPO.CheckForLedges(-hit.normal))
        {
            animator.SetBool("Pushing", true);
            velocity = direction * pushSpeed;
            charController.Move(velocity * Time.deltaTime);
            boxPO.Move(velocity);
        }
        else
        {
            boxPO.StopPlayingSound();
            animator.SetBool("Pushing", false);
            delegateList.RemoveAt(0);
        }
    }

    private void TriggerClimbUpAnimation()
    {
        animator.SetBool("ClimbUp", true);
    }

    private void TriggerClimbDownAnimation()
    {
        animator.SetBool("ClimbDown", true);
    }

    private void RemoveDelegate()
    {
        delegateList.RemoveAt(0);
    }

    public void WalkingSound()
    {
        GetComponent<MoreAudioClips>().PlayRandomClip();
    }
}