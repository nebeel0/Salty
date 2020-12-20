using UnityEngine;
using System.Collections;

public class ClickManager : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);
            GameObject fontPixel = hit.transform.gameObject;
            DynamicFontPixel stringChar = fontPixel.GetComponent<DynamicFontPixel>();
            stringChar.Toggle();
        }
    }

}