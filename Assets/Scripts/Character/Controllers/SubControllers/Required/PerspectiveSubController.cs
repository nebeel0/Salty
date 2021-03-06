using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
namespace Controller
{
    public class PerspectiveSubController : SubController
    {
        public Camera PrimaryCamera
        {
            get
            {
                return Player.primaryCamera;
            }
        }
        
        public Vector3 primaryCameraRootPosition = new Vector3(0, 0.5f, 0); //TODO static element?
        public bool thirdPerson;

        private void Start()
        {
            if(PrimaryCamera.transform.localPosition == primaryCameraRootPosition)
            {
                thirdPerson = false; //Determine this based off current position.
            }
            else
            {
                thirdPerson = true;
            }
        }

        private void Update()
        {
            UpdateCameraOffset();
        }

        public void OnTogglePerspective()
        {
            if (!enabled || IsGhost())
            {
                return;
            }
            thirdPerson = !thirdPerson;
            UpdateCameraOffset();
            Player.transform.localEulerAngles = Vector3.zero;
        }

        public void UpdateCameraOffset()
        {
            if(GetCluster() != null)
            {
                transform.localPosition = Vector3.zero;
                if (thirdPerson) //default third person
                {
                    float displacement = 4;
                    PrimaryCamera.transform.localPosition = primaryCameraRootPosition + Vector3.back * displacement;
                }
                else
                {
                    PrimaryCamera.transform.localPosition = primaryCameraRootPosition;
                }
                if(GetCluster().blocks.Count == 1)
                {
                    gameObject.transform.localScale = Vector3.one;
                }
                else
                {
                    gameObject.transform.localScale = Vector3.one * GetCluster().diagonal; //TODO we can get the exact math for size to diagonal
                }
            }
            else
            {
                gameObject.transform.localScale = Vector3.one;
                PrimaryCamera.transform.localPosition = primaryCameraRootPosition;
                thirdPerson = false;
            }
        }

    }
}
