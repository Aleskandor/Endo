using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    public Transform charPic;
    public bool speechOver =false;

    private bool sentenceOver;
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("AButton")) && sentenceOver)
            DisplayNextSentence();

    }
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        foreach (Transform child in charPic)
        {
            if (child.gameObject.name == dialogue.name)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
        }

        speechOver = false;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        sentenceOver = false;
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.00000000000000000000000000000001f);
        }
        sentenceOver = true;
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        speechOver = true;
    }

}
