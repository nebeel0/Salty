using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFontLetter : MonoBehaviour
{
    public string letter = "z";
    public GameObject FontPixelRef;
    Dictionary<string, GameObject> pixels = new Dictionary<string, GameObject>(); //5*7
    Dictionary<string, List<Vector2>> dynamicLetters = new Dictionary<string, List<Vector2>>()
    {
        ["a"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-1.5f, -2.5f), new Vector2(-1.5f, 0.5f), new Vector2(-1.5f, 1.5f), new Vector2(-0.5f, -2.5f), new Vector2(-0.5f, 1.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -2.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f) },
        ["b"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f) },
        ["c"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["d"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f) },
        ["e"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, 2.5f) },
        ["f"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, 2.5f) },
        ["g"] = new List<Vector2>() { new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -1.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, -1.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f) },
        ["h"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -0.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["i"] = new List<Vector2>() { new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -2.5f), new Vector2(-0.5f, -1.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, 1.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, 2.5f) },
        ["j"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, -2.5f), new Vector2(0.5f, -1.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, 2.5f) },
        ["k"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -0.5f), new Vector2(-0.5f, -1.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, -2.5f), new Vector2(0.5f, 1.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, 2.5f) },
        ["l"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-0.5f, -3.5f), new Vector2(0.5f, -3.5f), new Vector2(1.5f, -3.5f) },
        ["m"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, 1.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 1.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["n"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, 0.5f), new Vector2(-1.5f, 1.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, -1.5f), new Vector2(0.5f, -0.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["o"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["p"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["q"] = new List<Vector2>() { new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -2.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f) },
        ["r"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -1.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f) },
        ["s"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["t"] = new List<Vector2>() { new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -2.5f), new Vector2(-0.5f, -1.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, 1.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, 2.5f) },
        ["u"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-0.5f, -3.5f), new Vector2(0.5f, -3.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["v"] = new List<Vector2>() { new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -2.5f), new Vector2(-1.5f, -1.5f), new Vector2(-0.5f, -3.5f), new Vector2(0.5f, -2.5f), new Vector2(0.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["w"] = new List<Vector2>() { new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, -1.5f), new Vector2(-2.5f, -0.5f), new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -2.5f), new Vector2(-0.5f, -1.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, -1.5f), new Vector2(1.5f, -0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["x"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -1.5f), new Vector2(-1.5f, 0.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -1.5f), new Vector2(0.5f, 0.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, -2.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["y"] = new List<Vector2>() { new Vector2(-2.5f, 0.5f), new Vector2(-2.5f, 1.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -0.5f), new Vector2(-1.5f, 0.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -2.5f), new Vector2(-0.5f, -1.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f), new Vector2(0.5f, 0.5f), new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
        ["z"] = new List<Vector2>() { new Vector2(-2.5f, -3.5f), new Vector2(-2.5f, -2.5f), new Vector2(-2.5f, 2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-1.5f, -1.5f), new Vector2(-1.5f, 2.5f), new Vector2(-0.5f, -3.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 2.5f), new Vector2(0.5f, -3.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 2.5f), new Vector2(1.5f, -3.5f), new Vector2(1.5f, 1.5f), new Vector2(1.5f, 2.5f) },
    };
    int xLen = 5;
    int yLen = 7;

    void Start()
    {
        float initX = -xLen/2.0f;
        float initY = -yLen / 2.0f;
        for (int i = 0; i < xLen; i++)
        {
            float currentX = initX + i;
            for(int ii = 0; ii < yLen; ii++)
            {
                float currentY = initY + ii;
                Vector2 currentPos = new Vector2(currentX, currentY);
                GameObject currentFontPixel = Instantiate(FontPixelRef, transform);
                DynamicFontPixel currentFontPixelBehavior = currentFontPixel.GetComponent<DynamicFontPixel>();
                currentFontPixelBehavior.Toggle();
                currentFontPixel.transform.localPosition = new Vector3(currentPos.x, currentPos.y, 0);
                pixels[currentPos.ToString()] = currentFontPixel;
            }
        }
        SetBlocksToLetter();
    }

    // Used to formulate the character
    void Update()
    {
        //ResetBlocks();
        //SetBlocksToLetter();
    }

    void ResetBlocks()
    {
        float initX = -xLen / 2.0f;
        float initY = -yLen / 2.0f;
        for (int i = 0; i < xLen; i++)
        {
            float currentX = initX + i;
            for (int ii = 0; ii < yLen; ii++)
            {
                float currentY = initY + ii;
                Vector2 currentPos = new Vector2(currentX, currentY);
                if (pixels[currentPos.ToString()].GetComponent<DynamicFontPixel>().On())
                {
                    pixels[currentPos.ToString()].GetComponent<DynamicFontPixel>().Toggle();
                }
            }
        }
    }

    void SetBlocksToLetter()
    {
        List<Vector2> onBlocks = dynamicLetters[letter.ToLower()];
        for (int i = 0; i < onBlocks.Count; i++)
        {
            Vector2 currentPos = onBlocks[i];
            DynamicFontPixel currentFontPixel = pixels[currentPos.ToString()].GetComponent<DynamicFontPixel>();
            currentFontPixel.Toggle();
        }
    }

    void GenerateLetter()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ResetBlocks();
            SetBlocksToLetter();
        }
    }

    void LogHitBlocks()
    {
        List<Vector2> hitBlocks = new List<Vector2>();
        float initX = -xLen / 2.0f;
        float initY = -yLen / 2.0f;
        for (int i = 0; i < xLen; i++)
        {
            float currentX = initX + i;
            for (int ii = 0; ii < yLen; ii++)
            {
                float currentY = initY + ii;
                Vector2 currentPos = new Vector2(currentX, currentY);
                if (pixels[currentPos.ToString()].GetComponent<DynamicFontPixel>().On())
                {
                    hitBlocks.Add(currentPos);
                }
            }
        }
        List<string> posString = new List<string>();
        foreach (Vector2 pos in hitBlocks)
        {
            posString.Add(string.Format("new Vector2({0}f,{1}f)", pos.x, pos.y));
        }
        Debug.Log(string.Join(", ", posString));
    }
}
