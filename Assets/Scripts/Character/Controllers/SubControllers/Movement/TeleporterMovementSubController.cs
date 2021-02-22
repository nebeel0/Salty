
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class TeleporterMovementSubController : MovementSubController
    {
        Vector3 directionAggregator = Vector3.zero;
        protected override void Update()
        {
            if(direction.Equals(Vector3.zero) && !directionAggregator.Equals(Vector3.zero))
            {
                gameObject.transform.position += gameObject.transform.TransformDirection(directionAggregator) * 10 * Time.deltaTime;
                directionAggregator = Vector3.zero;
            }
            else
            {
                directionAggregator += direction;
            }

        }
    }
}
