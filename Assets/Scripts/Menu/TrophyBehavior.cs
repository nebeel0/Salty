using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyBehavior : MonoBehaviour
{
    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angles = transform.localEulerAngles;
        angles.y += Time.deltaTime * rotateSpeed;
        angles.z += Time.deltaTime * rotateSpeed * 2;
        transform.localEulerAngles = angles;
    }
}
