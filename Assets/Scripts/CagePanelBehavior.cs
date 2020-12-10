using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CagePanelBehavior : MonoBehaviour
{
    //TODO be able to control colors from parent
    //TODO toggle higher resolution
    //TODO if same color as gameObject interacting with it, bounce.
    //TODO if opposite color disinintegrate

    public int resolution;
    float coolDown;
    int coolDownTime = 120;
    float fadeOutPeriod;
    float fadeOutPeriodMax = 10;
    Color panelColor;
    Color currentColor;

    string[] panelTypes = {"",""};

    void Start()
    {
        coolDown = 0;
        transform.localScale = new Vector3(resolution/10.0f, 1, resolution/10.0f);  //plane is already 10x normal scale
        Loading();
    }

    void Update()
    {
        VisualUpdate();
    }

    void Loading()
    {
        Material panelMaterial = GetComponent<MeshRenderer>().material;
        coolDown = Random.Range(1, 5);
        panelColor = Color.clear;
        GetComponent<MeshRenderer>().enabled = false;
        currentColor = panelColor;
        panelMaterial.SetColor("_BaseColor", panelColor);
        panelMaterial.SetColor("_EmissionColor", panelColor);
    }

    void VisualUpdate()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        else if(currentColor.a < 0.01)
        {
            coolDown = RandomCoolDownTime();
            panelColor = Color.clear;
            if (Random.Range(0,10) == 0)
            {
                panelColor = RandomColor();
                GetComponent<MeshRenderer>().enabled = true;
            }
            currentColor = panelColor;
            fadeOutPeriod = Random.Range(1, fadeOutPeriodMax);
        }
        Material panelMaterial = GetComponent<MeshRenderer>().material;
        Color interpolatedColor = Color.Lerp(panelMaterial.GetColor("_BaseColor"), Color.clear, Time.deltaTime);
        Color interpolatedColorEmit = Color.Lerp(panelMaterial.GetColor("_EmissionColor"), currentColor, Time.deltaTime);
        panelMaterial.SetColor("_BaseColor", interpolatedColor);
        panelMaterial.SetColor("_EmissionColor", interpolatedColorEmit);
        FadeOutUpdate();
    }

    protected void FadeOutUpdate()
    {
        if (currentColor.a > 0.01)
        {
            currentColor = Color.Lerp(currentColor, Color.clear, Time.deltaTime/fadeOutPeriod);
        }
        else
        {
            currentColor = panelColor;
        }
    }

    float RandomCoolDownTime()
    {
        return Random.Range(coolDownTime / 2, coolDownTime);
    }

    Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }

}
