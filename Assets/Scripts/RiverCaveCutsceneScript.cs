using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class RiverCaveCutsceneScript : MonoBehaviour
{
    public GameObject human;
    public GameObject dog;
    public DialogueManager DM;
    public DialogueTrigger firstDT;
    public DialogueTrigger SecondDT;
    public Transform humanPoint;
    public Transform dogPoint;
    public Transform desiredCameraTransform;

    private Delegate tempDelegate;
    private List<Delegate> delegateList;
    private delegate void Delegate();

    private float transitionSpeed;
    private bool firstDialogue, secondDialogue, over, cameraMove;
    

    void Start()
    {
        delegateList = new List<Delegate>();

        transitionSpeed = 1f;
    }

    
    void Update()
    {
        if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);

        if (cameraMove)
            MoveCamera();


        if (!over)
        {
            DM = DM.GetComponent<DialogueManager>();
            if (firstDialogue)
            {
                firstDT.TriggerDialogue();
                firstDialogue = false;
                secondDialogue = true;
            }
            else if (secondDialogue && !firstDialogue && DM.speechOver)
            {
                SecondDT.TriggerDialogue();
                tempDelegate = new Delegate(DogRunToPoint);
                delegateList.Add(tempDelegate);
                secondDialogue = false;
                over = true;
            }
        }
        else
        {
            tempDelegate = new Delegate(HumanRunToPoint);
            delegateList.Add(tempDelegate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Human")
        {
            cameraMove = true;

            tempDelegate = new Delegate(Deactivate);
            delegateList.Add(tempDelegate);
            tempDelegate = new Delegate(TurnTowardsDog);
            delegateList.Add(tempDelegate);
        }
    }

    private void Deactivate()
    {
        //Turns off Human Logic     
        human.gameObject.GetComponent<Animator>().SetBool("Running", false);

        human.gameObject.GetComponent<CharacterController>().enabled = false;
        human.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        human.gameObject.GetComponent<HumanMovement>().enabled = false;
        human.gameObject.GetComponent<HumanAI>().enabled = false;

        //Turns off Dog Logic
        dog.gameObject.GetComponent<CharacterController>().enabled = false;
        dog.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        dog.gameObject.GetComponent<DogMovement>().enabled = false;
        dog.gameObject.GetComponent<DogAI>().enabled = false;

        delegateList.RemoveAt(0);
    }
    private void DogRunToPoint()
    {
        if (Vector3.Distance(dog.transform.position, dogPoint.position) > .3f)
        {

            if (Vector3.Distance(dog.transform.position, dogPoint.position) <= .3f)
                dog.gameObject.GetComponent<Animator>().SetBool("Running", false);
            else
                dog.gameObject.GetComponent<Animator>().SetBool("Running", true);

            dog.transform.position = Vector3.MoveTowards(dog.transform.position, dogPoint.position, 10f * Time.deltaTime);
            dog.transform.LookAt(dogPoint.transform);

 
        }
        else
            delegateList.RemoveAt(0);
    }
    private void HumanRunToPoint()
    {
        if (Vector3.Distance(human.transform.position, humanPoint.position) > .3f)
        {
            human.gameObject.GetComponent<Animator>().SetBool("Running", true);

            human.transform.position = Vector3.MoveTowards(human.transform.position, humanPoint.position, 8f * Time.deltaTime);
            human.transform.LookAt(humanPoint.transform);
        }
        else
        {
            human.gameObject.GetComponent<Animator>().SetBool("Running", false);
            human.gameObject.GetComponent<CharacterController>().enabled = true;
            delegateList.RemoveAt(0);
        }
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
    private void TurnTowardsDog()
    {
        Vector3 humanVector = dog.transform.position - human.transform.position;
        humanVector.Normalize();
        Vector3 desiredHumanRot = new Vector3(humanVector.x, 0, humanVector.z);
        human.gameObject.GetComponent<Animator>().SetBool("Running", false);

        if (Vector3.Angle(human.transform.forward, desiredHumanRot) > 2)
        {
            Quaternion humanLookRot = Quaternion.LookRotation(desiredHumanRot);
            human.transform.rotation = Quaternion.Lerp(human.transform.rotation, humanLookRot, 2f * Time.deltaTime);
        }
        else
        {
            firstDialogue = true;
            delegateList.RemoveAt(0);
        }
    }
}
