using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private Time globalTime;
    private float startTime;
    public float time;
    private bool isRunning;
    private bool isPaused;

    public void RunFromZero()
    {
        isRunning = true;
        isPaused = false;
        startTime = Time.realtimeSinceStartup;
        time = 0;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning && !isPaused)
            time = Time.realtimeSinceStartup - startTime;
        else if (isPaused)
        {
           startTime += Time.deltaTime;
        }
    }
}
