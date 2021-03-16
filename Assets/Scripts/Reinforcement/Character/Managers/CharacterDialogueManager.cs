using Character.Managers.Base;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Character.Managers
{
    public class CharacterDialogueManager : CharacterManagerBehavior
    {
        public bool initiateDialogue;

        private void Update()
        {
            if (initiateDialogue)
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
                Character.thoughts.Enqueue(new DialogueAction("hello world", new List<string>() { "no", "yes" }));
            }
            //TODO replace with some cool AI
        }

        public void InitiateDialogue()
        {
            Character.gameMaster.menuManager.dialogueManager.SetSpeaker(this);
        }




    }
}