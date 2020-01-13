using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    private bool play;

    public GameObject Human;
    public GameObject UI;
    public GameObject TPVcam;
    public GameObject Fcam;

    // Start is called before the first frame update
    void Start()
    {
        play = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.M))
        {
            if(play)
            {
                Human.SetActive(false);
                UI.SetActive(false);
                TPVcam.SetActive(false);
                Fcam.SetActive(true);
                play = false;
            }
            else if (!play)
            {
                Human.SetActive(true);
                UI.SetActive(true);
                TPVcam.SetActive(true);
                Fcam.SetActive(false);
                play = true;
            }
        }
    }
}
