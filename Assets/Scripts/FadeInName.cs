using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInName : MonoBehaviour
{
    static int nameCounter = 0;
    private float timer = 0;
    static Animator[] animators = null;
    GameObject sceneChangerGO;
    private bool finished = false;

    // Start is called before the first frame update
    void Start()
    {
        if (animators == null)
        {
            animators = new Animator[6];
            animators[0] = GameObject.Find("MadeBy").GetComponent<Animator>();
            animators[1] = GameObject.Find("NamnSimon").GetComponent<Animator>();
            animators[2] = GameObject.Find("NamnAnton").GetComponent<Animator>();
            animators[3] = GameObject.Find("NamnAlexander").GetComponent<Animator>();
            animators[4] = GameObject.Find("NamnJohn").GetComponent<Animator>();
            animators[5] = GameObject.Find("NamnAdrian").GetComponent<Animator>();
        }

        sceneChangerGO = GameObject.Find("SceneChanger");
    }

    public void Update()
    {
        if (finished)
        {
            BeginFadeOut();
        }
    }

    private void BeginFadeOut()
    {
        timer += Time.deltaTime;

        if (timer > 4)
        {
            sceneChangerGO.GetComponent<SceneChanger>().FadeToScene(0);
        }      
    }

    public void FadeInNextName()
    {
        if (nameCounter < animators.Length)
        {
            animators[nameCounter].SetTrigger("FadeOut");
            nameCounter++;
        }
        else
        {
            finished = true;
        }
    }
}
