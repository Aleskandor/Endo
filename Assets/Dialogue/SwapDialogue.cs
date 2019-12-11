using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    public DialogueTrigger karl1DT;
    public DialogueTrigger dogDT;
    public DialogueTrigger karl2DT;
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
                karl1DT.TriggerDialogue();
            }
            else if (part1Speech && startSpeech && !part2Speech && DM.speechOver)
            {
                part2Speech = true;
                dogDT.TriggerDialogue();
            }
            else if (part2Speech && startSpeech && DM.speechOver)
            {
                karl2DT.TriggerDialogue();
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
    }
}
