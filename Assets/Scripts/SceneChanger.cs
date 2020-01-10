using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int sceneToLoad;
    private bool playing = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            FadeToNextScene();

        if (Input.GetKeyDown(KeyCode.Y))
            FadeToPreviousScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Human")
            FadeToNextScene();
    }

    public void FadeToNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene < SceneManager.sceneCountInBuildSettings - 1)
        {
            FadeToScene(currentScene + 1);
        }
        else
        {
            FadeToScene(0);
        }
    }

    public void FadeToPreviousScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene > 0)
        {
            FadeToScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else
        {
            FadeToScene(SceneManager.sceneCountInBuildSettings - 1);
        }
    }

    public void FadeToScene(int sceneIndex)
    {
        if (SoundManager.instance != null && !playing)
        {
            playing = true;
            SoundManager.instance.Play("Transition");
            SoundManager.instance.PlayScene(sceneIndex);
        }
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
        playing = false;
    }
}
