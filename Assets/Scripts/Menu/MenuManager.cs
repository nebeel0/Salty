using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public GameObject pressAnyKeyRef;
    public GameObject startMenuRef;
    public Camera camera;

    void Start()
    {
        transform.position = camera.transform.position;
        LoadPreStartMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPreStartMenu()
    {
        Instantiate(pressAnyKeyRef, canvas.gameObject.transform);
    }

    public void LoadStartMenu()
    {
        Debug.Log("Start Menu");
    }
}
