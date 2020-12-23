using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class WorldSpaceGUITransform : MonoBehaviour
{
    // Object centers are sometimes in the bottom corner, use the point gui system to debug
    // TODO shrink title for upright phone position, but easier thing to do is lock the game in landscape mode.
    Camera cam;
    Vector2 screenResolution = Vector2.zero;
    public Vector2 screenAspect;
    public bool sideOriented = false;
    public bool top = true;
    public bool right = true;
    public float xPaddingFactor;
    public float yPaddingFactor;
    public float width; //TODO autocalculate this
    public float height; //TODO autocalculate this

    [ReadOnly]
    public float screenWidth; //TODO autocalculate this
    [ReadOnly]
    public float screenHeight; //TODO autocalculate this

    void Start()
    {
        cam = Camera.main;
    }

    void OnGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;

        Vector2 mousePos = new Vector2();
        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 50));
        Vector3 screenPosition = cam.WorldToScreenPoint(transform.position);
        Vector3 xScreenPos = cam.WorldToScreenPoint(new Vector3(transform.position.x + (width / 2), transform.position.y, transform.position.z));
        Vector3 yScreenPos = cam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + (height / 2), transform.position.z));

        //Debug.Log("screenPosition: " + screenPosition.ToString());
        //Debug.Log("xPosition: " + xScreenPos.ToString());
        //Debug.Log("yPosition: " + yScreenPos.ToString());

        GUILayout.BeginArea(new Rect(20, 20, 1000, 500));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
    }

    void Update()
    {
        Vector3 screenPosition = cam.WorldToScreenPoint(transform.position);
        Vector3 xScreenPos = cam.WorldToScreenPoint(new Vector3(transform.position.x + (width / 2), transform.position.y, transform.position.z));
        Vector3 yScreenPos = cam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + (height / 2), transform.position.z));
        screenWidth = xScreenPos.x - screenPosition.x;
        screenHeight = yScreenPos.y - screenPosition.y;

        Vector2 currentScreenResolution = new Vector2(cam.pixelWidth, cam.pixelHeight);
        screenResolution = currentScreenResolution;
        Vector3 point;
        if (!sideOriented)
        {
            point = cam.ScreenToWorldPoint(new Vector3(screenResolution.x * screenAspect.x, screenResolution.y * screenAspect.y, transform.position.z));
        }
        else
        {
            float xDisplacement = xScreenPos.x - (screenResolution.x - xPaddingFactor * screenResolution.x);
            float yDisplacement = yScreenPos.y - (screenResolution.y - yPaddingFactor * screenResolution.y);
            screenPosition.x -= xDisplacement;
            screenPosition.y -= yDisplacement;
            point = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, transform.position.z));
        }
        transform.position = new Vector3(point.x, point.y, transform.position.z);
    }

}