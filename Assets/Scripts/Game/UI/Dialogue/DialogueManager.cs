using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : CustomMenuBehavior
{
    //Manages loading title sequences, and character introductions
    public DialogueBehavior titleIntroPanel;
    public DialogueBehavior characterIntroPanel;

    public GameObject dialoguePanel;
    public SpeakerBehavior speakerPanel;
    public DialogueBehavior dialogueTextPanel;
    public DialogueResponseBehavior dialogueResponsePanel;

    public CharacterBehavior currentSpeaker;
    public DialogueAction currentDialogueAction;

    public override void Start()
    {
        if(currentSpeaker == null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public override void Death()
    {
        base.Death();
    }

    public override void PressButton(GameObject button)
    {
        Debug.LogError("This method shouldn't be called.");
    }

    public void SendResponse(string response)
    {
        currentSpeaker.Respond(response);
        ContinueDialogue();
    }

    public void SetSpeaker(CharacterBehavior character)
    {
        if(character.thoughts.Count > 0)
        {
            currentSpeaker = character;
            speakerPanel.SetCharacter(currentSpeaker);
            if (!dialoguePanel.activeSelf)
            {
                SetToLoadInPanel(dialoguePanel);
                LoadInPanel();
            }
            ContinueDialogue();
        }
    }

    public void ContinueDialogue()
    {
        //TODO implement a queue of messages, or a response system, if no responses, close the dialogue box.
        if(currentSpeaker.thoughts.Count > 0)
        {
            currentDialogueAction = currentSpeaker.thoughts.Dequeue();
            dialogueTextPanel.TypeMessage(message: currentDialogueAction.dialogue);
            dialogueResponsePanel.SetOptions(currentDialogueAction.responses);
        }
        else
        {
            LoadOutPanel();
        }
    }

}
