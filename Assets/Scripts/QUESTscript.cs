using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QUESTscript : MonoBehaviour
{
    public Transform Box;

    public void FindEndo()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "0")
                child.gameObject.SetActive(true);
        }
    }

    public void Humming()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "1")
                child.gameObject.SetActive(true);
        }
    }

    public void Kindness()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "2")
                child.gameObject.SetActive(true);
        }
    }

    public void Soul()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "3")
                child.gameObject.SetActive(true);
        }
    }
    public void Tracks()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "4")
                child.gameObject.SetActive(true);
        }
    }

    public void FindEndoDone()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "0done")
                child.gameObject.SetActive(true);
        }
    }

    public void HummingDone()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "1done")
                child.gameObject.SetActive(true);
        }
    }

    public void KindnessDone()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "2done")
                child.gameObject.SetActive(true);
        }
    }

    public void SoulDone()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "3done")
                child.gameObject.SetActive(true);
        }
    }
    public void TracksDone()
    {
        foreach (Transform child in Box)
        {
            if (child.gameObject.name == "4done")
                child.gameObject.SetActive(true);
        }
    }
}
