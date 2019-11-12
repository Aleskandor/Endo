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

    private float distanceToClimb;
    private float distanceClimbed;
    private float distanceToWalk;
    private float distanceWalked;

    private float currentWalkSpeed;
    private float originalWalkSpeed;
    private float currentSpeed;

    private float climbSpeed;
    private float gravity;

    private float turnSmoothTime;
    private float turnSmoothVelocity;
    private float speedSmoothVelocity;
    private float speedSmoothTime;

    private float velocityY;
    private float pushSpeed;

    public bool Locked;

    private void Start()
    {
        animator = GetComponent<Animator>();
        hit = new RaycastHit();
        delegateList = new List<Delegate>();
        camTransform = Camera.main.transform;
        charController = GetComponent<CharacterController>();

        distanceToClimb = 0;
        distanceClimbed = 0;
        distanceToWalk = 0;
        distanceWalked = 0;

        currentWalkSpeed = 6;
        originalWalkSpeed = 6;
        pushSpeed = originalWalkSpeed / 2;

        climbSpeed = 4;
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

            if (delegateList.Count != 0)
                delegateList[0].Method.Invoke(this, null);
            else
                Move(inputDir);

            if (Input.GetKeyDown(KeyCode.R))
                CheckForClimb();

            if (Input.GetKeyDown(KeyCode.F))
                CheckForPush();

            CheckforDrop();
        }
        else
            Locked = false;
    }

    private void Move(Vector2 inputDir)
    {
        animator.SetBool("Running", true);

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            animator.SetBool("Running", false);
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
        if (delegateList.Count == 0)
        {
            RaycastHit straightHit;
            RaycastHit overLedgeHit;

            int layerMask = 1 << 8;
            int lookDist = 100;

            // The acceptable distance we can be away from the wall in our forward plane when we raycast to detect a wall
            float acceptableDist = 2f;

            // We cannot climb up over objects shorter than this value from where we shoot our ray
            float minClimbHeight = 19f;

            // We can only climb on objects taller than this (it's a smaller value than minClimbHeight seeing as we
            // are shooting a ray from above and it will collide earlier on a higher object
            float maxClimbHeight = 18f;

            if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2), transform.forward, out straightHit, lookDist, layerMask))
            {
                if (straightHit.distance < acceptableDist)
                {
                    if (Physics.Raycast(transform.position + (transform.forward * 1.1f) + (transform.up * charController.height * 5),
                        -transform.up, out overLedgeHit, lookDist, layerMask))
                    {
                        // We are not able to climb up over anything which is closer from our raycast start than maxClimbHeight
                        // And we are not able to climb anything farther away from our raycast start than minClimbHeight
                        if (overLedgeHit.distance > maxClimbHeight && overLedgeHit.distance < minClimbHeight)
                        {
                            hit = straightHit;
                            distanceToClimb = 0.25f + overLedgeHit.point.y - transform.position.y;
                            distanceToWalk = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(hit.point.x, hit.point.z)) + charController.radius;

                            // Add the order of events that will comprise this action
                            tempDelegate = new Delegate(TurnTowardsWall);
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
                    currentWalkSpeed = 0;

                    if (Input.GetKeyDown(KeyCode.R) && Physics.Raycast(transform.position + (transform.forward * 1f) + (-transform.up * 1),
                        -transform.forward, out hit, lookDist, layerMask))
                    {
                        distanceToWalk = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(hit.point.x, hit.point.z)) + charController.radius;

                        tempDelegate = new Delegate(TurnAwayFromWall);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(WalkForwards);
                        delegateList.Add(tempDelegate);
                        tempDelegate = new Delegate(ClimbDown);
                        delegateList.Add(tempDelegate);
                    }
                }
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
            float acceptableDist = 2f;

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

    private void TurnAwayFromWall()
    {
        if (Vector3.Angle(transform.forward, hit.normal) > 2)
        {
            Quaternion lookRotation = Quaternion.LookRotation(hit.normal);

            // Rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }
        else
        {
            delegateList.RemoveAt(0);
        }
    }

    private void ClimbDown()
    {
        if (!charController.isGrounded)
        {
            Vector3 velocity = -transform.up * climbSpeed;
            charController.Move(velocity * Time.deltaTime);
        }
        else
            delegateList.RemoveAt(0);
    }

    private void TriggerClimbUpAnimation()
    {
        animator.SetBool("Climbing", true);
    }

    private void RemoveDelegate()
    {
        delegateList.RemoveAt(0);
    }

    private void ClimbUp()
    {
        animator.SetBool("Climbing", false);
        transform.position += Vector3.up * distanceToClimb + transform.forward * distanceToWalk;
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
}