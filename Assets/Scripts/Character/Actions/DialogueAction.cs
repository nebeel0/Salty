using System.Collections.Generic;


public class DialogueAction
{
    //actions
    //information dictionary
    //network of other character behaviors
    //goals
    public CharacterBehavior target;
    public string dialogue;
    public List<string> responses = new List<string>();
    public List<Action> concurrentActions;

    public DialogueAction(string dialogue, List<string> responses=null)
    {
        this.dialogue = dialogue;
        if(responses != null)
        {
            this.responses = responses;
        }
    }


    //Can trigger a response from any or all the characters involved in the conversation
    //Can trigger another character to talk
    //Can trigger prompt from the user.


}
