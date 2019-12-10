﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhistleEndlessStartDialog : MonoBehaviour
{
    private DialogueTrigger DT;

    void Start()
    {
        DT = gameObject.GetComponent<DialogueTrigger>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DT.TriggerDialogue();
        }
    }
}