using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiedStartDialogue : MonoBehaviour
{
    private DialogueTrigger DT;
    private bool speechOver = false;
    public bool replayable = false;
    void Start()
    {
        DT = gameObject.GetComponent<DialogueTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.name == "Human" && !speechOver) || (other.name == "Dog" && !speechOver))
        {
            speechOver = true;
            DT.TriggerDialogue();
        }
    }
}
