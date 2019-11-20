using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int sceneToLoad;

    public Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            FadeToNextScene();

        if (Input.GetKeyDown(KeyCode.Y))
            FadeToPreviousScene();
    }

    public void FadeToNextScene()
    {
        FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FadeToPreviousScene()
    {
        FadeToScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void FadeToScene(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
