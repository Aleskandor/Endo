using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritScript : MonoBehaviour
{
    public GameObject head_J;
    public GameObject human;
    public DialogueManager DM;

    Quaternion startRotation, currentRotation;
    float speedFactor;
    bool humanClose;
    DialogueTrigger DT;

    // Start is called before the first frame update
    void Start()
    {
        speedFactor = 1;
        startRotation = transform.localRotation;
        DT = gameObject.GetComponent<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
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
                DT.TriggerDialogue();
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
}
