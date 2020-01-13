using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOverRiver : MonoBehaviour
{
    public bool overRiver;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Human")
            overRiver = true;
    }
}
