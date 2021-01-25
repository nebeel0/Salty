using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class DialogueBehavior : MonoBehaviour
{
    public TMP_Text dialogueText;
    public AudioClip typeSound1;
    bool typingLock;

    public void TypeMessage(string message, float letterPause = 0.02f)
    {
        if(!typingLock)
        {
            dialogueText.text = "";
            StartCoroutine(TypeLetter(message, letterPause));
        }
    }


    IEnumerator TypeLetter(string message, float letterPause)
    {
        typingLock = true;
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterPause);
        }
        typingLock = false;
    }
}
