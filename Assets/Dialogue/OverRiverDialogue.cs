using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverRiverDialogue : MonoBehaviour
{
    public GameObject logManager;

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
            logManager.GetComponent<QUESTscript>().Tracks();
            DT.TriggerDialogue();
        }
    }
}
