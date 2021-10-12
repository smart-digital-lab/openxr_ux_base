using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Framerate : MonoBehaviour
{
    private int numFrames = 10;
    private TextMeshPro textDisplay;
    private float[] frameRates;
    private int frameCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        textDisplay = GetComponent<TextMeshPro>();
        frameRates = new float[numFrames];       
    }

    // Update is called once per frame
    void Update()
    {
        if (textDisplay != null) 
        {
            frameRates[frameCounter] = 1.0f / Time.unscaledDeltaTime;
            frameCounter = (frameCounter + 1) % numFrames;

            textDisplay.text = Average().ToString();
        }
    }

    int Average()
    {
        float sum = 0.0f;
        for (int i=0; i<numFrames; i++)
        {
            sum += frameRates[i];
        }
        return (Mathf.RoundToInt(sum/numFrames));
    }
}
