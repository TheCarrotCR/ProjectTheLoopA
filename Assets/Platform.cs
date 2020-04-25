using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool passable;

    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.GetComponent<ClimbyThing>() != null)
            passable = true;
    }
}
