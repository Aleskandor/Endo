using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiritScript : MonoBehaviour
{
    public GameObject head_J;
    public GameObject human;
    public GameObject dog;
    public GameObject orb;
    public DialogueManager DM;
    public Transform humanPoint;
    public Transform dogPoint;
    public Transform desiredCameraTransform;
    public Transform karlHand;
    public GameObject logManager;

    private Delegate tempDelegate;
    private List<Delegate> delegateList;
    private delegate void Delegate();

    private Transform previousCamPos;
    private Quaternion startRotation, currentRotation;
    private float speedFactor, transitionSpeed;
    private bool humanClose;
    private DialogueTrigger DT;

    private Vector3 orbStart, orbEnd;
    private bool running, orbCaught;
    private float arcHeight, orbSpeed;
    public bool animationOver;

    // Start is called before the first frame update
    void Start()
    {
        speedFactor = 1;
        transitionSpeed = .6f;
        orbSpeed = 3;
        arcHeight = 4;
        startRotation = transform.localRotation;
        DT = gameObject.GetComponent<DialogueTrigger>(); 
        delegateList = new List<Delegate>();
        animationOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        orbEnd = karlHand.position;

        if (orbCaught)
            orb.transform.position = karlHand.position;

        if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);

        if (humanClose)
        {
            head_J.transform.LookAt(human.transform);
            head_J.transform.Rotate(0, 0, -90);
            head_J.transform.Rotate(90, 0, 0);
        }
        else if (!humanClose)
        {
            head_J.transform.localRotation = Quaternion.Lerp(head_J.transform.localRotation, Quaternion.identity, Time.deltaTime);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Human")
        {
            humanClose = true;
            if (!DM.speechOver)
            {
                tempDelegate = new Delegate(RunToPoint);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(TurnTowardsSpirit);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(TriggerDialogue);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(SpawnOrb);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(OrbFromSpirit);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(MoveOrb);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(GetOrb);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(PlayAnimation);
                delegateList.Add(tempDelegate);
                tempDelegate = new Delegate(Reactivate);
                delegateList.Add(tempDelegate);

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Human")
        {
            humanClose = false;
        }
    }

    private void Deactivate()
    {
        //Turns off Human Logic     
        human.gameObject.GetComponent<CharacterController>().enabled = false;
        human.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        human.gameObject.GetComponent<HumanMovement>().enabled = false;
        human.gameObject.GetComponent<HumanAI>().enabled = false;

        //Turns off Dog Logic
        dog.gameObject.GetComponent<CharacterController>().enabled = false;
        dog.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        dog.gameObject.GetComponent<DogMovement>().enabled = false;
        dog.gameObject.GetComponent<DogAI>().enabled = false;


    }

    private void PlayAnimation()
    {
        if (animationOver)
        {
            delegateList.RemoveAt(0);
        }
    }

    private void Reactivate()
    {
        orb.SetActive(false);

        Camera.main.gameObject.GetComponent<ThirdPersonView>().enabled = true;
        human.gameObject.GetComponent<HumanMovement>().enabled = true;
        human.gameObject.GetComponent<CharacterController>().enabled = true;

        dog.gameObject.GetComponent<DogAI>().enabled = true;
        dog.gameObject.GetComponent<NavMeshAgent>().enabled = true;

        human.gameObject.GetComponent<Animator>().SetTrigger("GetOrbFinished");
        logManager.GetComponent<QUESTscript>().Soul();
        delegateList.RemoveAt(0);
    }

    private void TurnTowardsSpirit()
    {
        Vector3 humanVector = transform.position - human.transform.position;
        humanVector.Normalize();
        Vector3 dogVector = transform.position - dog.transform.position;
        dogVector.Normalize();
        Vector3 desiredHumanRot = new Vector3(humanVector.x, 0, humanVector.z);
        Vector3 desiredDogRot = new Vector3(dogVector.x, 0, dogVector.z);
        human.gameObject.GetComponent<Animator>().SetBool("Running", false);
        dog.gameObject.GetComponent<Animator>().SetBool("Running", false);

        if (Vector3.Angle(human.transform.forward, desiredHumanRot) > 2 || Vector3.Angle(dog.transform.forward, desiredDogRot) > 2)
        {
            Quaternion humanLookRot = Quaternion.LookRotation(desiredHumanRot);
            human.transform.rotation = Quaternion.Lerp(human.transform.rotation, humanLookRot, 2f * Time.deltaTime);

            Quaternion dogLookRot = Quaternion.LookRotation(desiredDogRot);
            dog.transform.rotation = Quaternion.Lerp(dog.transform.rotation, dogLookRot, 2f * Time.deltaTime);
        }
        else
            delegateList.RemoveAt(0);
    }
    private void RunToPoint()
    {
        if (Vector3.Distance(human.transform.position, humanPoint.position) > .3f || Vector3.Distance(dog.transform.position, dogPoint.position) > .3f)
        {
            Deactivate();
            human.gameObject.GetComponent<Animator>().SetBool("Running", true);
            

            if(Vector3.Distance(dog.transform.position, dogPoint.position) <= .3f)
                dog.gameObject.GetComponent<Animator>().SetBool("Running", false);
            else
                dog.gameObject.GetComponent<Animator>().SetBool("Running", true);

            if (Vector3.Distance(human.transform.position, humanPoint.position) <= .3f)
                human.gameObject.GetComponent<Animator>().SetBool("Running", false);
            else
                human.gameObject.GetComponent<Animator>().SetBool("Running", true);

            human.transform.position = Vector3.MoveTowards(human.transform.position, humanPoint.position, 5f * Time.deltaTime);
            human.transform.LookAt(humanPoint.transform);
            dog.transform.position = Vector3.MoveTowards(dog.transform.position, dogPoint.position, 8f * Time.deltaTime);
            dog.transform.LookAt(dogPoint.transform);

            MoveCamera();
        }
        else
            delegateList.RemoveAt(0);
    }

    private void MoveCamera()
    {
        Camera.main.gameObject.GetComponent<ThirdPersonView>().enabled = false;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredCameraTransform.position, transitionSpeed * Time.deltaTime);

        Vector3 currentAngle = new Vector3(
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, desiredCameraTransform.rotation.eulerAngles.x, transitionSpeed * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, desiredCameraTransform.rotation.eulerAngles.y, transitionSpeed * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, desiredCameraTransform.rotation.eulerAngles.z, transitionSpeed * Time.deltaTime));

        Camera.main.transform.eulerAngles = currentAngle;
    }

    private void TriggerDialogue()
    {
        DT.TriggerDialogue();
        delegateList.RemoveAt(0);
    }

    private void MoveOrb()
    {
        Vector2 pos = new Vector2(orbStart.x, orbStart.z);
        Vector2 posTarget = new Vector2(orbEnd.x, orbEnd.z);
        float distance = Vector2.Distance(posTarget, pos);
        Vector2 nextPlanePos = Vector2.MoveTowards(new Vector2(orb.transform.position.x, orb.transform.position.z), posTarget, orbSpeed * Time.deltaTime);

        float baseY = Mathf.Lerp(orbStart.y, orbEnd.y, Vector2.Distance(nextPlanePos, pos) / distance);
        float arc = arcHeight * Vector2.Distance(nextPlanePos, pos) * Vector2.Distance(nextPlanePos, posTarget) / (0.25f * distance * distance);
        Vector3 nextPos = new Vector3(nextPlanePos.x, baseY + arc, nextPlanePos.y);
        orb.transform.position = nextPos;

        if (nextPos == orbEnd)
        {
            nextPos = karlHand.position;
            orbCaught = true;
            delegateList.RemoveAt(0);
        }
    }

    private void OrbFromSpirit()
    {
        human.GetComponent<Animator>().SetTrigger("OrbFromSpirit");
        delegateList.RemoveAt(0);
    }
    private void GetOrb()
    {
        human.gameObject.GetComponent<Animator>().SetTrigger("GetOrb");
        //human.GetComponent<Animator>().SetBool("Idle", true);
        delegateList.RemoveAt(0);
        
        
    }

    void SpawnOrb()
    {
        if (DM.speechOver)
        {
            orb.SetActive(true);
            orbStart = orb.transform.position;
            delegateList.RemoveAt(0);
        }
    }
}
