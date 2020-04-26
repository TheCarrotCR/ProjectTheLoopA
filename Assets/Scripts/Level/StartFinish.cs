using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFinish : MonoBehaviour
{
    public enum ThisState { Start, Finish }

    public ThisState state;

    public void PerformBehavior() 
    {
        if (state == ThisState.Start)
            PerformStartBehavior();
        else
            PerformFinishBehavior();
    }

    private void PerformStartBehavior() 
    {

    }

    private void PerformFinishBehavior() 
    {

    }
}
