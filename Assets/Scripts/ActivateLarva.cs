using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateLarva : MonoBehaviour
{
    public GameObject Stick;
    public GameObject Human;
    public GameObject UI;
    public GameObject MainCamera;
    public GameObject puzzleCamera;

    private bool inDaZone = false;
    private bool inDaPuzzle = false;

    void Update()
    {
        if (inDaPuzzle && LarvaInteract.HasCrossed)
        {
            Stick.SetActive(false);
            MainCamera.SetActive(true);
            puzzleCamera.SetActive(false);
            Human.SetActive(true);
            //Human.GetComponent<CharacterController>().enabled = true;
            //Human.GetComponent<HumanMovement>().enabled = true;
            inDaPuzzle = false;
        }
        else if (inDaZone && !LarvaInteract.HasCrossed && FindPrintsDialog.over)
        {
            Stick.SetActive(true);
            MainCamera.SetActive(false);
            puzzleCamera.SetActive(true);
            Human.SetActive(false);
            //Human.GetComponent<CharacterController>().enabled = false;
            //Human.GetComponent<HumanMovement>().enabled = false;
            inDaPuzzle = true;
            //Human.GetComponent<Animator>().SetBool("Running", false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Human")
        {
            inDaZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Human")
        {
            inDaZone = false;
        }
    }
}
