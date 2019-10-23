using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ThirdPersonView : MonoBehaviour
{
    private Vector2 pitchMinMax;
    private Vector3 rotationSmoothVelocity;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private Vector3 cameraPosition;
    private Vector3 newCamPosition;

    private GameObject other;

    private RaycastHit hit;

    private CharacterMovement targetCM;
    private CharacterController targetCC;
    private FollowerAI otherFAI;
    private NavMeshAgent otherNVA;

    public GameObject target;
    public GameObject Human;
    public GameObject Dog;

    private float lerpTime;
    private float currentLerpTime;

    private float minAngleDiffForSwap;
    private float maxSwapDistance;

    private float yaw;
    private float pitch;

    private float distance;
    private float minDistance;
    private float maxDistance;
    
    private float mouseSensitivity;
    private float dstFromTarget;
    private float rotationSmoothTime;

    private bool justSwitched;
    private bool swappingTarget;

    public bool lockCursor;

    private void Start()
    {
        lerpTime = 3;
        currentLerpTime = 0;
        minAngleDiffForSwap = 10;
        maxSwapDistance = 6;

        justSwitched = false;
        swappingTarget = false;

        pitchMinMax = new Vector2(-5, 55);

        minDistance = 1.0f;
        maxDistance = 4.0f;

        mouseSensitivity = 10;
        dstFromTarget = 7.5f;
        rotationSmoothTime = 0.12f;

        target = Human;
        other = Dog;

        targetCM = target.GetComponent<CharacterMovement>();
        targetCC = target.GetComponent<CharacterController>();
        otherFAI = other.GetComponent<FollowerAI>();
        otherNVA = other.GetComponent<NavMeshAgent>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !swappingTarget)
        {
            swappingTarget = true;
        }

        if (swappingTarget)
            SwapTarget();
    }

    void LateUpdate()
    {
        if (!swappingTarget)
        {
            if (target == Human)
                cameraPosition = GameObject.Find("HumanPivot").transform.position;
            else
                cameraPosition = GameObject.Find("DogPivot").transform.position;

            if (!justSwitched)
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                targetRotation = new Vector3(pitch, yaw);
            }
            else
                justSwitched = false;                

            currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;
            transform.position = cameraPosition - transform.forward * dstFromTarget;

            CompensateForCollision();
        }
    }

    void CompensateForCollision()
    {
        Debug.DrawLine(cameraPosition, transform.position, Color.red);

        if (Physics.Linecast(cameraPosition, transform.position, out hit))
        {
            if (hit.collider.name != "Main Camera" && hit.collider.name != "Player" && hit.collider.name != "Dog" && hit.collider.name != "Cylinder" && 
                hit.collider.name != "BoundingBox")
            {
                distance = Mathf.Clamp((hit.distance), minDistance, maxDistance);
                newCamPosition = cameraPosition - transform.forward * distance;
                transform.position = new Vector3(hit.point.x, newCamPosition.y, hit.point.z);
            }
        }
    }

    void SwapTarget()
    {
        if (Vector3.Distance(target.transform.position, other.transform.position) < maxSwapDistance)
        {
            targetCM.enabled = false;
            targetCC.enabled = false;
            otherFAI.enabled = false;
            otherNVA.enabled = false;

            float targetFacing = Vector3.Angle((other.transform.position - target.transform.position), target.transform.forward);
            float otherFacing = Vector3.Angle((target.transform.position - other.transform.position), other.transform.forward);

            if (targetFacing > minAngleDiffForSwap)
            {
                Vector3 dirFromTargetToOther = other.transform.position - target.transform.position; dirFromTargetToOther.Normalize();
                dirFromTargetToOther.y = 0f;
                Quaternion lookRotationDog = Quaternion.LookRotation(dirFromTargetToOther);
                target.transform.rotation = Quaternion.Lerp(target.transform.rotation, lookRotationDog, Time.deltaTime * 2);
            }

            if (otherFacing > minAngleDiffForSwap)
            {
                Vector3 dirFromOtherToTarget = target.transform.position - other.transform.position; dirFromOtherToTarget.Normalize();
                dirFromOtherToTarget.y = 0f;
                Quaternion lookRotationHuman = Quaternion.LookRotation(dirFromOtherToTarget);
                other.transform.rotation = Quaternion.Lerp(other.transform.rotation, lookRotationHuman, Time.deltaTime * 2);
            }

            if (targetFacing < minAngleDiffForSwap && otherFacing < minAngleDiffForSwap)
            {
                Vector3 newCamTargetPos = other.transform.position - other.transform.forward * dstFromTarget;

                if (currentLerpTime >= 1)
                {
                    if (target == Dog)
                    {
                        target = Human;
                        other = Dog;
                    }
                    else
                    {
                        target = Dog;
                        other = Human;
                    }

                    targetCM = target.GetComponent<CharacterMovement>();
                    targetCC = target.GetComponent<CharacterController>();
                    otherFAI = other.GetComponent<FollowerAI>();
                    otherNVA = other.GetComponent<NavMeshAgent>();

                    Vector3 relativePos = target.transform.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

                    pitch = rotation.eulerAngles.x;
                    yaw = rotation.eulerAngles.y;

                    targetRotation = rotation.eulerAngles;
                    currentRotation = rotation.eulerAngles;

                    targetCM.enabled = true;
                    targetCC.enabled = true;
                    otherFAI.enabled = true;
                    otherNVA.enabled = true;
                    swappingTarget = false;
                    justSwitched = true;
                    currentLerpTime = 0;
                }

                else
                {
                    Vector3 tempCamTargetPos = target.transform.position + (target.transform.forward * (Vector3.Distance(target.transform.position, other.transform.position) / 2));
                    currentLerpTime += Time.deltaTime;
                    transform.LookAt(tempCamTargetPos);
                    transform.position = Vector3.Lerp(transform.position, newCamTargetPos, currentLerpTime / lerpTime);
                    targetRotation = tempCamTargetPos;
                }
            }
        }
        else
        {
            swappingTarget = false;
        }
    }

    public GameObject GetCurrentTarget()
    {
        return target;
    }
}