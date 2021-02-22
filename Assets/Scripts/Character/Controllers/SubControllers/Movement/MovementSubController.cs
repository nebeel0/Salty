using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class MovementSubController : SubController
    {
        public Vector3 direction = Vector3.zero;

        public void OnMove(InputValue inputValue)
        {
            direction.x = inputValue.Get<Vector2>().x;
            direction.z = inputValue.Get<Vector2>().y;
        }

        protected virtual void Update()
        {
            Debug.LogError("Override Me.");
        }

    }
}


