using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumMaterialBehavior : MonoBehaviour
{
    protected float fadeOutPeriod; //10 default
    protected float coolDown;
    public bool allOn;
    public bool grayScale;
    public int coolDownTime;  //120 default
    public int loadingTimeMax; // 60 default
    public float fadeOutPeriodMax;
    public int randomChance;

    public float fadeInPeriodMax = 10;
    public float fadeInPeriodMin = 10;
    float fadeInPeriod;
    bool fadingIn = true;
    bool colliding;


    Color panelColor;
    Color currentColor;

    

    protected virtual void Start()
    {
        coolDown = 0;
        Material panelMaterial = GetComponent<MeshRenderer>().material;
        coolDown = Random.Range(1, loadingTimeMax);
        panelColor = Color.clear;
        GetComponent<MeshRenderer>().enabled = false;
        currentColor = panelColor;
        panelMaterial.SetColor("_BaseColor", panelColor);
    }

    protected virtual void Update()
    {
        if(colliding)
        {
            Color lightWhite = new Color(0.2f, 0.2f, 0.2f, 1);
            Material panelMaterial = GetComponent<MeshRenderer>().material;
            panelMaterial.SetColor("_BaseColor", lightWhite);
            GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            VisualUpdate();
        }
    }

    private void OnCollisionEnter()
    {
        colliding = true;
    }

    private void OnCollisionStay()
    {
        colliding = true;
    }

    private void OnCollisionExit()
    {
        colliding = false;
    }

    protected void VisualUpdate()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        else if (currentColor.a < 0.01)
        {
            RandomReset();
        }
        if (GetComponent<MeshRenderer>().enabled == true)
        {
            if (fadingIn)
            {
                FadeInUpdate();
            }
            else
            {
                FadeOutUpdate();
            }
        }
    }

    protected void RandomReset()
    {
        coolDown = RandomCoolDownTime();
        panelColor = Color.clear;
        if (Random.Range(0, randomChance) == 0 || allOn)
        {
            if (grayScale)
            {
                panelColor = Color.white;
            }
            else
            {
                panelColor = RandomColor();
            }
            GetComponent<MeshRenderer>().enabled = true;
            fadingIn = true;
            fadeInPeriod = Random.Range(fadeInPeriodMin, fadeOutPeriodMax);
            currentColor = panelColor;
        }
        else
        {
            fadingIn = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
        fadeOutPeriod = Random.Range(1, fadeOutPeriodMax);
    }

    protected void FadeInUpdate()
    {
        Material panelMaterial = GetComponent<MeshRenderer>().material;
        if (panelMaterial.GetColor("_BaseColor").a < panelColor.a - 0.5f)
        {
            Color interpolatedColor = Color.Lerp(panelMaterial.GetColor("_BaseColor"), panelColor, Time.deltaTime / fadeInPeriod);
            panelMaterial.SetColor("_BaseColor", interpolatedColor);
        }
        else
        {
            fadingIn = false;
        }
    }

    protected void FadeOutUpdate()
    {
        Material panelMaterial = GetComponent<MeshRenderer>().material;
        Color interpolatedColor = Color.Lerp(panelMaterial.GetColor("_BaseColor"), currentColor, Time.deltaTime);

        panelMaterial.SetColor("_BaseColor", interpolatedColor);
        if (currentColor.a > 0.01)
        {
            currentColor = Color.Lerp(currentColor, Color.clear, Time.deltaTime / fadeOutPeriod);
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
        Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        return newColor;
    }

}
