using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialoguePanelResponseBehavior : CustomMenuBehavior
{
    public DialoguePanelManager dialogueManager;
    public CustomButtonBehavior[] options;
    public CustomButtonBehavior customOptionButton;

    public GameObject defaultOptionsPanel;
    public GameObject customOptionPanel;

    public TMP_InputField customUserText;
    public GameObject customBackButton;
    public GameObject customSendButton;

    public string[] randomPlaceholders =
    {
        "...Type something...",
        "...Write whatever...", 
        "...you won't be creative :D..."
    };

    public void SetOptions(List<string> options)
    {
        if(options.Count == 0)
        {
            this.options[0].SetText("...");
        }
        else
        {
            int maxOptions = System.Math.Min(3, options.Count);
            for (int i = 0; i < maxOptions; i++)
            {
                this.options[i].SetText(options[i]);
                this.options[i].gameObject.SetActive(true);
            }
        }
        for (int i = 2; i > System.Math.Max(options.Count-1,0) ; i--)
        {
            this.options[i].gameObject.SetActive(false);
        }
        customOptionButton.SetText(randomPlaceholders[Random.Range(0, randomPlaceholders.Length)]);
        customOptionButton.gameObject.SetActive(true);

        SetToLoadInPanel(defaultOptionsPanel);
        customOptionPanel.SetActive(false);
    }

    public override void PressButton(GameObject button)
    {
        if(button == customOptionButton.gameObject)
        {
            SetToLoadInPanel(customOptionPanel);
        }
        else if (button == customBackButton)
        {
            SetToLoadInPanel(defaultOptionsPanel);
        }
        else if (button == customSendButton)
        {
            dialogueManager.SendResponse(customUserText.text);
        }
        else
        {
            dialogueManager.SendResponse(button.GetComponent<CustomButtonBehavior>().GetText());
        }
    }
}
