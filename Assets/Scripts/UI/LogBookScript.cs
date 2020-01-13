using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBookScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Log;
    public GameObject Cont;
    public Animator animator;

    private bool open;
    private bool tab;

    void Start()
    {
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("SelectButton")) && !open)
        {
            animator.SetBool("IsOpen", true);
            Log.SetActive(true);
            Cont.SetActive(false);
            open = true;
        }
        else if ((Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("SelectButton")) && open)
        {
            animator.SetBool("IsOpen", false);
            open = false;
        }

        if((Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("XButton")) && open && tab)
        {
            Log.SetActive(false);
            Cont.SetActive(true);
            tab = false;
        }
        else if((Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("XButton")) && open && !tab)
        {
            Log.SetActive(true);
            Cont.SetActive(false);
            tab = true;
        }
    }
}
