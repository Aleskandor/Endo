using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHelp : MonoBehaviour
{
    public bool help;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Human")
            help = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Human")
            help = false;
    }
}
