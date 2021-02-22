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

    public PlayerController Player
    {
        get { return Character.Player; }
    }

    public GhostController Ghost
    {
        get { return Character.Player.Ghost; }
    }

    public ClusterBehavior Cluster
    {
        get { return Character.Cluster; }
    }
}
