using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Character.Managers;

public class DialoguePanelManager : CustomMenuBehavior
{
    //Manages loading title sequences, and character introductions
    public DialoguePanelBehavior titleIntroPanel;
    public DialoguePanelBehavior characterIntroPanel;

    public GameObject dialoguePanel;
    public DialoguePanelSpeakerBehavior speakerPanel;
    public DialoguePanelBehavior dialogueTextPanel;
    public DialoguePanelResponseBehavior dialogueResponsePanel;

    public CharacterDialogueManager currentSpeaker;
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

    public void SetSpeaker(CharacterDialogueManager speaker)
    {
        if(speaker.Character.thoughts.Count > 0)
        {
            currentSpeaker = speaker;
            speakerPanel.SetCharacter(speaker.Character);
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
        if(currentSpeaker.Character.thoughts.Count > 0)
        {
            currentDialogueAction = currentSpeaker.Character.thoughts.Dequeue();
            dialogueTextPanel.TypeMessage(message: currentDialogueAction.dialogue);
            dialogueResponsePanel.SetOptions(currentDialogueAction.responses);
        }
        else
        {
            LoadOutPanel();
        }
    }

}
