using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CharacterBehavior : GameBehavior
{
    //actions
    //information dictionary
    //network of other character behaviors
    //goals
    public string primaryAlias;
    public HashSet<string> aliases;
    public Queue<DialogueAction> thoughts = new Queue<DialogueAction>();
    public bool initiateDialogue;

    public override void Start()
    {
        base.Start();
        thoughts.Enqueue(new DialogueAction("hello world", new List<string>() { "no", "yes" }));
    }

    private void Update()
    {
        if(initiateDialogue)
        {
            initiateDialogue = false;
            InitiateDialogue();
        }
    }

    public void Respond(string response)
    {
        bool responseEqual = response.Equals("yes");
        if (responseEqual)
        {
            thoughts.Enqueue(new DialogueAction("hello world", new List<string>() { "no", "yes" }));
        }
        //TODO replace with some cool AI
    }

    public void InitiateDialogue()
    {
        gameMaster.menuManager.dialogueManager.SetSpeaker(this);
    }


}
