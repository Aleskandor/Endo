using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBookScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Text text;
    public GameObject Log;
    public GameObject Cont;
    public Animator animator;

    private bool open;
    private bool tab;

    void Start()
    {
        open = false;
        AddToLog("- Do More Stuff");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !open)
        {
            animator.SetBool("IsOpen", true);
            Log.SetActive(true);
            Cont.SetActive(false);
            open = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && open)
        {
            animator.SetBool("IsOpen", false);
            open = false;
        }

        if(Input.GetKeyDown(KeyCode.Tab) && open && tab)
        {
            Log.SetActive(false);
            Cont.SetActive(true);
            tab = false;
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && open && !tab)
        {
            Log.SetActive(true);
            Cont.SetActive(false);
            tab = true;
        }
    }

    public void AddToLog(string t)
    {
        text.text = text.text + "\n" + t;
    }
}
