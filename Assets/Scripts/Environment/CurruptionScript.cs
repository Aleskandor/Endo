using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurruptionScript : MonoBehaviour
{
    Animator ac;
    float offSetAni;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        ac = this.GetComponent<Animator>();
        offSetAni = Random.Range(0f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= offSetAni)
            ac.SetBool("Idle", true);
    }
}
