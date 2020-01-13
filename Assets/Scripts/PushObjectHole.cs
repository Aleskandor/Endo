using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObjectHole : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pushable")
        {
            PushObject PO = other.GetComponent<PushObject>();

            PO.inHole = true;
        }
    }
}
