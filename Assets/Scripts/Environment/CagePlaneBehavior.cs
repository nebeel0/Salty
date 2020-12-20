using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CagePlaneBehavior : MonoBehaviour
{
    public int randomChance;
    public int coolDownTime;
    public float fadeOutPeriodMax;
    public int loadingTimeMax;
    public bool grayScale;
    public bool allOn;
    public GameObject cagePanelRef;
    // Start is called before the first frame update
    Vector2 dimensions = new Vector2(300, 300);
    int resolution = 10;

    void Start()
    {
        int panelsX = (int) dimensions.x / resolution;
        int panelsY = (int) dimensions.y / resolution;

        for(int row=0; row< panelsX; row++)
        {
            float currX = -dimensions.x / 2.0f + resolution / 2.0f + resolution*row;
            for (int column=0; column< panelsY; column++)
            {
                float currY = -dimensions.y / 2.0f + resolution / 2.0f + resolution* column;
                Vector3 panelPosition = new Vector3(currX, 0, currY); //y is actually z
                GameObject instatiatedCagePanel = Instantiate(cagePanelRef, transform);
                CagePanelBehavior cagePanelBehavior = instatiatedCagePanel.GetComponent<CagePanelBehavior>();
                cagePanelBehavior.coolDownTime = coolDownTime;
                cagePanelBehavior.fadeOutPeriodMax = fadeOutPeriodMax;
                cagePanelBehavior.grayScale = grayScale;
                cagePanelBehavior.randomChance = randomChance;
                cagePanelBehavior.loadingTimeMax = loadingTimeMax;
                cagePanelBehavior.allOn = allOn;
                cagePanelBehavior.resolution = resolution;
                instatiatedCagePanel.transform.localPosition = panelPosition;
            }
        }
    }

    public void SetDimensions(Vector2 dimensions)
    {
        this.dimensions = dimensions;
    }

    public void SetResolution(int resolution)
    {
        this.resolution = resolution;
    }
}
