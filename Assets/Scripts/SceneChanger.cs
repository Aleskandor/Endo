using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int sceneToLoad;

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
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
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
