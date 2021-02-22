using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
namespace Controller
{
    public class PerspectiveController : SubController
    {
        public Camera PrimaryCamera
        {
            get
            {
                if(transform.childCount != 1)
                {
                    Debug.LogError("Should only have one child object which is the camera.");
                }
                return transform.GetChild(0).GetComponent<Camera>();
            }
        }
        
        public Vector3 primaryCameraRootPosition = new Vector3(0, 0.5f, 0); //TODO static element?
        bool perspectiveFlag;


        private void Update()
        {
            UpdateCameraOffset();
        }

        void OnTogglePerspective()
        {
            if (!enabled)
            {
                return;
            }
            perspectiveFlag = !perspectiveFlag;
            PrimaryCamera.transform.localPosition = primaryCameraRootPosition;
        }


        public void UpdateCameraOffset()
        {
            if(Player.Cluster != null)
            {
                transform.position = Cluster.trackingBlock.transform.position;
                if (perspectiveFlag) //default third person
                {
                    float displacement = Mathf.Max(Cluster.diagonal * 4, 4);
                    PrimaryCamera.transform.localPosition = primaryCameraRootPosition + Vector3.back * displacement;
                }
                else
                {
                    PrimaryCamera.transform.localPosition = primaryCameraRootPosition;
                }
            }
            else
            {
                PrimaryCamera.transform.localPosition = primaryCameraRootPosition;
            }

        }

    }
}
