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
        public float thirdPersonCameraDisplacement = 4;
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

        void OnTogglePerspective()
        {
            if (!enabled || IsGhost() || GetComponent<RotationSubController>().cameraZoomMode)
            {
                return;
            }
            thirdPerson = !thirdPerson;
            UpdateCameraOffset();
            transform.localEulerAngles = Vector3.zero;
        }

        void UpdateCameraOffset()
        {
            Vector3 newScale;
            Vector3 newCameraPosition;
            if (GetCluster() != null)
            {
                if (thirdPerson) //default third person
                {
                    newCameraPosition = primaryCameraRootPosition + Vector3.back * thirdPersonCameraDisplacement;
                }
                else
                {
                    newCameraPosition = primaryCameraRootPosition;
                }
                if(GetCluster().blocks.Count == 1)
                {
                    newScale = Vector3.one;
                }
                else
                {
                    newScale = Vector3.one * GetCluster().diagonal; //TODO we can get the exact math for size to diagonal
                }
            }
            else
            {
                newScale = Vector3.one;
                newCameraPosition = primaryCameraRootPosition;
                thirdPerson = false;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime);
            PrimaryCamera.transform.localPosition = Vector3.Lerp(PrimaryCamera.transform.localPosition, newCameraPosition, Time.deltaTime * 10);
        }

    }
}
