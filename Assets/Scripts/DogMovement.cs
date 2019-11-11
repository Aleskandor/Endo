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

    private Vector3 direction;
    
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

        currentWalkSpeed = 10;
        originalWalkSpeed = 10;
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

            if (Input.GetKeyDown(KeyCode.F))
                CheckForPush();

            CheckforDrop();
        }
        else
            Locked = false;
    }

    private void Move(Vector2 inputDir)
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
                        tempDelegate = new Delegate(Push);
                        delegateList.Add(tempDelegate);
                    }
                }
            }
        }
    }

    private void Push()
    {
        if (Input.GetKey(KeyCode.F))
        {
            velocity = direction * pushSpeed;
            charController.Move(velocity * Time.deltaTime);
            hit.collider.gameObject.GetComponent<PushObject>().Move(velocity);
        }
        else
        {
            delegateList.RemoveAt(0);
        }
    }
}