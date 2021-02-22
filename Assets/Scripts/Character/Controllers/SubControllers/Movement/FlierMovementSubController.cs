
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class FlierMovementSubController : MovementSubController
    {
        protected override void Update()
        {
            gameObject.transform.position += gameObject.transform.TransformDirection(direction) * 10 * Time.deltaTime;
        }
    }
}
