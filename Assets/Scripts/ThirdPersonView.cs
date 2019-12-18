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

    private HumanMovement humanMovement;
    private DogMovement dogMovement;

    private HumanAI humanAI;
    private DogAI dogAI;

    private CharacterController targetCC;
    private NavMeshAgent otherNVA;

    private Transform humanPivot;
    private Transform dogPivot;
    private Transform targetPivot;
    private Transform otherPivot;

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
        Camera camera = Camera.main;
        float[] distances = new float[32];
        distances[10] = 150;
        camera.layerCullDistances = distances;

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

        humanPivot = GameObject.Find("HumanPivot").transform;
        dogPivot = GameObject.Find("DogPivot").transform;

        targetPivot = humanPivot;
        otherPivot = dogPivot;

        humanMovement = Human.GetComponent<HumanMovement>();
        dogMovement = Dog.GetComponent<DogMovement>();

        humanAI = Human.GetComponent<HumanAI>();
        dogAI = Dog.GetComponent<DogAI>();

        targetCC = target.GetComponent<CharacterController>();
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
                cameraPosition = humanPivot.position;
            else if (target == Dog)
                cameraPosition = dogPivot.position;

            if (!justSwitched)
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

                yaw += Input.GetAxis("RightStickHorizontal") * mouseSensitivity;
                pitch -= Input.GetAxis("RightStickVertical") * mouseSensitivity;
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
        if (Vector3.Distance(target.transform.position, other.transform.position) <= maxSwapDistance)
        {
            if (target == Human)
            {
                humanMovement.enabled = false;
                dogAI.enabled = false;
            }
            else if (target == Dog)
            {
                dogMovement.enabled = false;
                humanAI.enabled = false;
            }

            targetCC.enabled = false;
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
                Vector3 newCamTargetPos = otherPivot.position - otherPivot.forward * dstFromTarget;

                if (currentLerpTime >= 1)
                {
                    if (target == Dog)
                    {
                        target = Human;
                        other = Dog;
                        targetPivot = humanPivot;
                        otherPivot = dogPivot;
                    }
                    else
                    {
                        target = Dog;
                        other = Human;
                        targetPivot = dogPivot;
                        otherPivot = humanPivot;
                    }

                    targetCC = target.GetComponent<CharacterController>();
                    otherNVA = other.GetComponent<NavMeshAgent>();

                    Vector3 relativePos = targetPivot.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

                    pitch = rotation.eulerAngles.x;
                    yaw = rotation.eulerAngles.y;

                    targetRotation = rotation.eulerAngles;
                    currentRotation = rotation.eulerAngles;

                    if (target == Human)
                    {
                        humanMovement.enabled = true;
                        dogAI.enabled = true;
                    }
                        
                    else if (target == Dog)
                    {
                        dogMovement.enabled = true;
                        humanAI.enabled = true;
                    }

                    targetCC.enabled = true;
                    otherNVA.enabled = true;
                    swappingTarget = false;
                    justSwitched = true;
                    currentLerpTime = 0;
                }
                else
                {
                    Vector3 tempCamTargetPos = otherPivot.position + (otherPivot.forward * (Vector3.Distance(otherPivot.position, targetPivot.position) / 2));
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

    public GameObject GetCurrentOther()
    {
        return other;
    }
}