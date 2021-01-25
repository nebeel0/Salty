using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class SpeakerBehavior : MonoBehaviour
{
    public TMP_Text speakerName;
    //TODO implement color based off the emotion covariance matrix between the two.
    //TODO implement images.

    public void SetCharacter(CharacterBehavior character)
    {
        speakerName.text = character.primaryAlias;
    }
}
