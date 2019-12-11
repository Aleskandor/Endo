using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushToClimbDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    public DialogueTrigger karlDT;
    public DialogueTrigger dogDT;
    public DialogueTrigger stoneDT;
    public DialogueManager DM;

    private bool startSpeech = false;
    private bool part1Speech = false;
    private bool part2Speech = false;

    public bool over = false;

    void Update()
    {
        if (!over)
        {
            DM = DM.GetComponent<DialogueManager>();

            if (!part1Speech && startSpeech)
            {
                part1Speech = true;
                karlDT.TriggerDialogue();
            }
            else if (part1Speech && startSpeech && DM.speechOver)
            {
                dogDT.TriggerDialogue();
                over = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.name == "Human" && !startSpeech) || (other.name == "Dog" && !startSpeech))
        {
            startSpeech = true;
        }
        else if (other.tag == "Pushable")
        {
            stoneDT.TriggerDialogue();
            over = true;
        }
    }
}
