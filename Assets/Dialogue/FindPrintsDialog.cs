using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPrintsDialog : MonoBehaviour
{
    public GameObject Human;

    public DialogueTrigger karl1DT;
    public DialogueTrigger karl2DT;
    public DialogueTrigger spiritDT;
    public DialogueManager DM;
    public GameObject light;
    public GameObject logManager;
    public Transform desiredCameraTransform;
    public Transform larvaTransform;

    private Delegate tempDelegate;
    private List<Delegate> delegateList;
    private delegate void Delegate();
    private int currentIntens;
    private int maxIntens;

    private bool startSpeech = false;
    private bool part1Speech = false;
    private bool part2Speech = false;
    private bool lightUp = false;
    private bool stop = false;

    private float transitionSpeed;

    public static bool over = false;

    void Start()
    {
        transitionSpeed = .9f;
        currentIntens = 0;
        maxIntens = 5000;
        delegateList = new List<Delegate>();
    }

    void Update()
    {
        if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);

        if (!over)
        {
            DM = DM.GetComponent<DialogueManager>();

            if (!part1Speech && startSpeech)
            {
                Deactivate();
                tempDelegate = new Delegate(MoveCamera);
                delegateList.Add(tempDelegate);

                part1Speech = true;
                karl1DT.TriggerDialogue();
            }
            else if (part1Speech && startSpeech && !part2Speech && DM.speechOver)
            {
                tempDelegate = new Delegate(TurnCameraTowardsLarva);
                delegateList.Add(tempDelegate);

                if (lightUp)
                {
                    tempDelegate = new Delegate(LightUp);
                    delegateList.Add(tempDelegate);

                    part2Speech = true;
                    spiritDT.TriggerDialogue();
                }
            }
            else if (part2Speech && startSpeech && DM.speechOver)
            {
                karl2DT.TriggerDialogue();
                over = true;
            }
        }
        else if (over && DM.speechOver && !stop)
        {
            Reactivate();
            logManager.GetComponent<QUESTscript>().Kindness();
            stop = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.name == "Human" && !startSpeech) || (other.name == "Dog" && !startSpeech))
        {
            startSpeech = true;
        }
    }

    private void MoveCamera()
    {
        Camera.main.gameObject.GetComponent<ThirdPersonView>().enabled = false;

        if (Vector3.Distance(Camera.main.transform.position, desiredCameraTransform.position) > .4f)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredCameraTransform.position, transitionSpeed * Time.deltaTime);

            Vector3 currentAngle = new Vector3(
                Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, desiredCameraTransform.rotation.eulerAngles.x, transitionSpeed * Time.deltaTime),
                Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, desiredCameraTransform.rotation.eulerAngles.y, transitionSpeed * Time.deltaTime),
                Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, desiredCameraTransform.rotation.eulerAngles.z, transitionSpeed * Time.deltaTime));

            Camera.main.transform.eulerAngles = currentAngle;
        }
        else
            delegateList.RemoveAt(0);
    }

    private void TurnCameraTowardsLarva()
    {

        Quaternion targetRotation = Quaternion.LookRotation(larvaTransform.position - Camera.main.transform.position);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, targetRotation, transitionSpeed * Time.deltaTime * 2);

        float diff = Camera.main.transform.rotation.eulerAngles.x - targetRotation.eulerAngles.x;
        float degree = 0.01f;

        if (Mathf.Abs(diff) <= degree)
        {
            delegateList.RemoveAt(0);
            lightUp = true;
            SoundManager.instance.Play("Magic");
        }
    }

    private void Deactivate()
    {
        Human.gameObject.GetComponent<CharacterController>().enabled = false;
        Human.gameObject.GetComponent<HumanMovement>().enabled = false;
        Human.gameObject.GetComponent<Animator>().SetBool("Running", false);
    }

    private void Reactivate()
    {
        Camera.main.gameObject.GetComponent<ThirdPersonView>().enabled = true;
        Human.gameObject.GetComponent<CharacterController>().enabled = true;
        Human.gameObject.GetComponent<HumanMovement>().enabled = true;
    }

    private void LightUp()
    {
        if (currentIntens <= maxIntens)
        {
            currentIntens +=500;
            light.GetComponent<Light>().intensity = currentIntens;
        }
        else
            delegateList.RemoveAt(0);
    }
}