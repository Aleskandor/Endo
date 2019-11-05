using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    PostProcessVolume pV;
    // Start is called before the first frame update
    void Start()
    {
        pV = GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pV.enabled && Input.GetKeyDown(KeyCode.P))
        {
            pV.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            pV.enabled = true;
        }
    }
}
