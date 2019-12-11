using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBookScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Text text;
    public Animator animator;

    private bool open;

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
            open = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && open)
        {
            animator.SetBool("IsOpen", false);
            open = false;
        }


    }

    public void AddToLog(string t)
    {
        text.text = text.text + "\n" + t;
    }
}
