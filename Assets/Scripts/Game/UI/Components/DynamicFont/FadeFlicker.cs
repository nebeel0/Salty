using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeFlicker : MonoBehaviour
{
    public float flickerRate;
    public float loadingOffset;
    public bool turnOff=false;

    public Color faceColor
    {
        get { return textMeshPro.faceColor; }
    }

    TMP_Text textMeshPro;
    bool fadeIn;

    void OnEnable()
    {
        Start();
    }

    // Start is called before the first frame update
    void Start()
    {
        flickerRate = 0.5f;
        loadingOffset = 1;
        turnOff = false;
        flickerRate = Mathf.Max(flickerRate, 1);
        textMeshPro = GetComponent<TMP_Text>();
        fadeIn = true;
        textMeshPro.faceColor = new Color(1,1,1,0); //transparent white
    }

    // Update is called once per frame
    void Update()
    {
        if (loadingOffset > 0)
        {
            loadingOffset -= Time.deltaTime;
        }
        else
        {
            Color currentColor = textMeshPro.faceColor;
            if (fadeIn && !turnOff)
            {
                if (currentColor.a > 0.9f)
                {
                    fadeIn = false;
                }
                currentColor.a += Time.deltaTime * flickerRate;
            }
            else
            {
                if (currentColor.a < 0.1f)
                {
                    fadeIn = true;
                }
                currentColor.a -= Time.deltaTime * flickerRate;
            }
            textMeshPro.faceColor = currentColor;
        }
    }

}
