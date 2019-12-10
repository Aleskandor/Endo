using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPrintsDialog : MonoBehaviour
{
    public DialogueTrigger karl1DT;
    public DialogueTrigger karl2DT;
    public DialogueTrigger spiritDT;
    public DialogueManager DM;
    public GameObject light;

    private bool startSpeech = false;
    private bool part1Speech = false;
    private bool part2Speech = false;
    private bool over = false;

    void Start()
    {

    }

    void Update()
    {
        if (!over)
        {
            DM = DM.GetComponent<DialogueManager>();

            if (!part1Speech && startSpeech && DM.speechOver)
            {
                part1Speech = true;
                karl1DT.TriggerDialogue();
            }
            else if (part1Speech && startSpeech && !part2Speech && DM.speechOver)
            {
                part2Speech = true;
                spiritDT.TriggerDialogue();
            }
            else if (part2Speech && startSpeech && DM.speechOver)
            {
                karl2DT.TriggerDialogue();
                over = true;
            }
        }
        else if (over)
            light.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.name == "Human" && !startSpeech) || (other.name == "Dog" && !startSpeech))
        {
            startSpeech = true;
        }
    }
}
