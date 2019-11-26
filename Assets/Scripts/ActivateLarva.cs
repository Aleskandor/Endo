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
        if (inDaPuzzle && Input.GetKeyDown(KeyCode.G))
        {
            Stick.SetActive(false);
            UI.SetActive(true);
            MainCamera.SetActive(true);
            puzzleCamera.SetActive(false);
            Human.GetComponent<CharacterController>().enabled = true;
            Human.GetComponent<HumanMovement>().enabled = true;
            inDaPuzzle = false;
        }
        else if (inDaZone && Input.GetKeyDown(KeyCode.G))
        {
            Stick.SetActive(true);
            UI.SetActive(false);
            MainCamera.SetActive(false);
            puzzleCamera.SetActive(true);
            Human.GetComponent<CharacterController>().enabled = false;
            Human.GetComponent<HumanMovement>().enabled = false;
            inDaPuzzle = true;
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
