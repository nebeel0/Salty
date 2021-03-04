using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CharacterManagerBehavior : MonoBehaviour
{
    public CharacterBehavior Character
    {
        get { return GetComponent<CharacterBehavior>(); }
    }

    public PlayerControlManager Player
    {
        get { return Character.Player; }
    }
}
