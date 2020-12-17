using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostPlayerController : Controller
{
    float ghostSpeed = 10;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        target = transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        GhostPositionUpdate();
    }

    protected void GhostPositionUpdate()
    {
        if(holdFlag)
        {
            Vector3 newPosition = transform.position + transform.forward * ghostSpeed;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime); //holdscalar is already created with time.delta
        }
    }

    
    
}
