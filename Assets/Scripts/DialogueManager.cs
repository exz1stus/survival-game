using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences;
    public Text dialogueText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DisplayNextSentence();
    }
    void Start()
    {
        sentences = new Queue<string>();
    }
    public void StartDialogue(InteractableObject obj)
    {
        sentences.Clear();
        foreach(string sentence in obj.description)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    void EndDialogue()
    {
        dialogueText.text = "";
    }
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
