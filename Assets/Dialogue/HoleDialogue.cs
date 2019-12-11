using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDialogue : MonoBehaviour
{
    public DialogueTrigger KarlDialogue;
    public DialogueTrigger DogDialogue;

    private ThirdPersonView tpv;
    private bool karlSpeechOver = false;
    private bool dogSpeechOver = false;

    void Start()
    {
        tpv = Camera.main.gameObject.GetComponent<ThirdPersonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Human" && tpv.target.name == "Human" && !karlSpeechOver)
        {
            karlSpeechOver = true;
            KarlDialogue.TriggerDialogue();
        }
        if (other.name == "Dog" && tpv.target.name == "Dog" && !dogSpeechOver)
        {
            dogSpeechOver = true;
            DogDialogue.TriggerDialogue();
        }
    }
}
