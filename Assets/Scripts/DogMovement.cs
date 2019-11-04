using UnityEngine;
using System;
using System.Collections.Generic;

public class DogMovement : MonoBehaviour
{
    [Range(0, 1)]
    private Delegate tempDelegate;
    private RaycastHit hit;
    private Transform camTransform;
    private CharacterController charController;
    private List<Delegate> delegateList;
    private delegate void Delegate();

    public Vector3 velocity;
    private float getOverLedgeTimer;
    private float distanceToClimb;
    private float distanceClimbed;
    private float distanceToBackUp;
    private float distanceBackedUp;

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

    public bool Locked;
    public bool pushing;

    public float pushSpeed;

    private void Start()
    {
        hit = new RaycastHit();
        delegateList = new List<Delegate>();
        camTransform = Camera.main.transform;
        charController = GetComponent<CharacterController>();

        getOverLedgeTimer = 0;
        distanceToClimb = 0;
        distanceClimbed = 0;
        distanceToBackUp = 0;
        distanceBackedUp = 0;

        currentWalkSpeed = 6;
        originalWalkSpeed = 6;
        pushSpeed = originalWalkSpeed / 2;

        climbSpeed = 4;
        gravity = -12;

        turnSmoothTime = 0.2f;
        speedSmoothTime = 0.1f;

        Locked = false;
        pushing = false;
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

            if (gameObject.name != "Dog" && Input.GetKeyDown(KeyCode.R))
            {
                CheckForClimb();
            }

            CheckforDrop();
        }
        else
            Locked = false;
    }

    private void Move(Vector2 inputDir)
    {
        if (!pushing)
        {
            if (inputDir != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
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
        else
            charController.Move(velocity * Time.deltaTime);
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
                        Debug.Log(overLedgeHit.distance);

                        // We are not able to climb up over anything which is closer from our raycast start than maxClimbHeight
                        // And we are not able to climb anything farther away from our raycast start than minClimbHeight
                        if (overLedgeHit.distance > maxClimbHeight && overLedgeHit.distance < minClimbHeight)
                        {
                            hit = straightHit;
                            distanceToClimb = /*charController.height / 2 + */overLedgeHit.point.y - transform.position.y;
                            
                            // Add the order of events that will comprise this action
                            tempDelegate = new Delegate(TurnTowardsWall);
                            delegateList.Add(tempDelegate);
                            tempDelegate = new Delegate(ClimbUp);
                            delegateList.Add(tempDelegate);
                            tempDelegate = new Delegate(WalkForwardsABit);
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

                    if (gameObject.name != "Dog")
                    {
                        if (Input.GetKeyDown(KeyCode.R) && Physics.Raycast(transform.position + (transform.forward * 1f) + (-transform.up * 1),
                            -transform.forward, out hit, lookDist, layerMask))
                        {
                            distanceToBackUp = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(hit.point.x, hit.point.z)) + charController.radius;

                            tempDelegate = new Delegate(TurnTowardsWall);
                            delegateList.Add(tempDelegate);
                            tempDelegate = new Delegate(WalkBackwardsABit);
                            delegateList.Add(tempDelegate);
                            tempDelegate = new Delegate(ClimbDown);
                            delegateList.Add(tempDelegate);
                        }
                    }                 
                }
                else if (minDistToGround < groundHit.distance && groundHit.distance > maxDistToGround)
                    currentWalkSpeed = 0;
                else
                    currentWalkSpeed = originalWalkSpeed;
            }
        }
    }

    private void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(horizontal, 0f, vertical) * currentWalkSpeed * Time.deltaTime;

        transform.Translate(playerMovement, Space.Self);
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

    private void ClimbUp()
    {
        if (distanceClimbed < distanceToClimb)
        {
            Vector3 velocity = transform.up * climbSpeed;
            charController.Move(velocity * Time.deltaTime);
            distanceClimbed += velocity.y * Time.deltaTime;
        }
        else
        {
            distanceClimbed = 0;
            delegateList.RemoveAt(0);
        }
    }

    private void WalkForwardsABit()
    {
        getOverLedgeTimer += Time.deltaTime;
        charController.Move(transform.forward * Time.deltaTime);

        if (getOverLedgeTimer > 1)
        {
            getOverLedgeTimer = 0;
            delegateList.RemoveAt(0);
        }
    }

    private void WalkBackwardsABit()
    {
        if (distanceBackedUp < distanceToBackUp)
        {
            distanceBackedUp += transform.forward.magnitude * Time.deltaTime;
            charController.Move(-transform.forward * Time.deltaTime);
        }
        else
        {
            distanceBackedUp = 0;
            delegateList.RemoveAt(0);
        }
    }
}