using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HumanMovement : MonoBehaviour
{
    [Range(0, 1)]
    private Delegate tempDelegate;
    private RaycastHit hit;
    private List<Delegate> delegateList;
    private delegate void Delegate();
    private MoreAudioClips[] moreClips;

    private CharacterController charController;
    private Transform camTransform;
    private Vector3 velocity;
    private Vector3 direction;
    private Vector3 pushTargetPos;
    private Vector3 charTargetPos;
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
        //moreClips is a list of the MoreAudioClips component. 
        //Index 0 should be the footsteps clips and index 1 should be the grunts
        moreClips = GetComponents<MoreAudioClips>();

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
            Vector2 input = new Vector2(Input.GetAxisRaw("LeftStickHorizontal") + Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("LeftStickVertical") + Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;

            if (delegateList.Count != 0)
                delegateList[0].Method.Invoke(this, null);
            else if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("AButton"))
            {
                CheckForClimb();
                CheckforDrop();
            }
            else if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("XButton"))
                CheckForPush();
            else if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("YButton"))
            {
                SoundManager.instance.PlayWhistle();
                GameObject dog = GameObject.Find("Dog");
                float delay = (Vector3.Distance(transform.position, dog.transform.position) / 150);
                if (delay < 0.5)
                {
                    delay = 1;
                }
                if(SceneManager.GetActiveScene().buildIndex != 2)
                    dog.GetComponent<MoreAudioClips>().PlayRandomClipDelayed(delay);
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

        if (!CheckforLedges())
        {
            charController.Move(velocity * Time.deltaTime);
            currentSpeed = new Vector2(charController.velocity.x, charController.velocity.z).magnitude;
        }

        if (charController.isGrounded)
        {
            velocityY = 0;
        }
        else
        {
            charController.Move(Vector3.up * velocityY * Time.deltaTime);
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
            RaycastHit hitClimb;

            int layerMask = 1 << 8;
            int lookDist = 100;
            float minDistToGround = 4;
            float maxDistToGround = 7;

            if (Physics.Raycast(transform.position, -transform.up, out hitClimb, Mathf.Infinity, layerMask))
            {
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
    }

    private bool CheckforLedges()
    {
        RaycastHit groundHit;

        int lookDist = 100;
        float minDistToGround = 4;

        if (Physics.Raycast(transform.position + (Vector3.up * charController.height / 2) + (transform.forward * 1f), Vector3.down, out groundHit, lookDist))
        {
            if (groundHit.distance > minDistToGround)
                return true;
            else
                return false;
        }
        else
            return false;
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

        if ((Input.GetKey(KeyCode.F) || Input.GetButton("XButton")) && !boxPO.CheckForObstacle(-hit.normal))
        {
            animator.SetBool("Pushing", true);

            pushTargetPos = boxPO.transform.position +( -hit.normal * 4f);
            charTargetPos = transform.position + (-hit.normal * 4f);

            delegateList.RemoveAt(0);
            tempDelegate = new Delegate(PushMove);
            delegateList.Add(tempDelegate);
        }
        else
        {
            animator.SetBool("Pushing", false);
            delegateList.RemoveAt(0);
            boxPO.GetComponent<CharacterController>().Move(Vector3.zero);
            boxPO.GetComponent<PushObject>().StopPlayingSound();
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
        if (moreClips[0])
        {
            moreClips[0].PlayRandomClip();
        }
    }

    public void GruntSound()
    {
        if (moreClips[1])
        {
            moreClips[1].PlayRandomClip();
        }
    }

    public void CoughSound()
    {
        if (moreClips[2])
        {
            moreClips[2].PlayRandomClip();
        }
    }

    public void SetAnimationOverTrue()
    {
        GameObject.FindGameObjectWithTag("ForestSpirit").GetComponent<SpiritScript>().animationOver = true;
    }

    private void PushMove()
    {
        PushObject boxPO = hit.collider.gameObject.GetComponent<PushObject>();
        boxPO.GetComponent<CharacterController>().enabled = false;

        transform.position = Vector3.MoveTowards(transform.position, charTargetPos, pushSpeed * Time.deltaTime);
        boxPO.transform.position = Vector3.MoveTowards(boxPO.transform.position, pushTargetPos, pushSpeed * Time.deltaTime);
        Debug.Log(boxPO.transform.position + " " + pushTargetPos);

        if(Math.Round(boxPO.transform.position.x, 2)== Math.Round(pushTargetPos.x, 2) && Math.Round(boxPO.transform.position.z, 2) == Math.Round(pushTargetPos.z, 2))
        {
            delegateList.RemoveAt(0);
            tempDelegate = new Delegate(Push);
            delegateList.Add(tempDelegate);
            boxPO.GetComponent<CharacterController>().enabled = true;
        }
    }
}