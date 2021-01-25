using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFontSentence : MonoBehaviour
{
    // Start is called before the first frame update
    public string sentence = "hello world";
    public GameObject dynamicFontLetterRef;
    public float space = 5 / 3f;
    public bool left;
    List<GameObject> letters = new List<GameObject>();

    void Start()
    {
        Vector3 initPos = Vector3.zero;
        sentence = left ? sentence : Reverse(sentence);
        for(int i=0; i < sentence.Length; i++)
        {
            float displaceMent = 5 * i + space * i;
            displaceMent = left ? displaceMent : -1 * displaceMent;
            Vector3 currentPos = new Vector3(initPos.x + displaceMent, 0, 0);
            if (sentence[i] != ' ')
            {
                letters.Add(CreateLetter(letter: sentence[i], position: currentPos));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject CreateLetter(char letter, Vector3 position)
    {
        GameObject letterObject = Instantiate(dynamicFontLetterRef, transform);
        DynamicFontLetter letterBehavior = letterObject.GetComponent<DynamicFontLetter>();
        letterBehavior.letter = letter.ToString();
        letterObject.transform.localPosition = position;
        return letterObject;
    }

    static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
