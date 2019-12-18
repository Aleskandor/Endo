using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasCrossedDialogue : MonoBehaviour
{
    private DialogueTrigger dt;
    private bool played;

    public GameObject logManager;

    // Start is called before the first frame update
    void Start()
    {
        dt = gameObject.GetComponent<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LarvaInteract.HasCrossed && !played)
        {
            dt.TriggerDialogue();
            logManager.GetComponent<QUESTscript>().KindnessDone();
            played = true;
        }
    }
}
