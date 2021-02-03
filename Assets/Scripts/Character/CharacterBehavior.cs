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
    public CharacterDialogueManager CharacterDialogueManager
    {
        get { return GetComponent<CharacterDialogueManager>(); }
    }
    public CharacterTriggerManager CharacterAutomationManager
    {
        get { return GetComponent<CharacterTriggerManager>(); }
    }
    public ClusterBehavior Cluster
    {
        get { return Player.cluster; }
    }
    public PlayerController Player
    {
        get { return GetComponent<PlayerController>(); }
    }
    public override void Start()
    {
        base.Start();
    } 
}
