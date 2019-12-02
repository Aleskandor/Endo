using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurruptionHoleInteraction : MonoBehaviour
{
    public bool die;

    public GameObject human;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (die)
        {            
            foreach (Transform cuppution in transform)
            {
                cuppution.gameObject.GetComponent<Animator>().SetBool("Dead", true);
            }
        } 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Human")
        {
            human.GetComponent<Animator>().SetTrigger("GiveOrb");
            die = true;
        }
    }
}
