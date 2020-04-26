using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbyThing : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom")
        {
            if (other.gameObject.GetComponent<Player>().state == Player.State.Climb)
            {
                var bounds = GetComponent<Collider2D>().bounds;
                var otherBounds = other.bounds;
                other.transform.Translate(new Vector2(bounds.center.x - otherBounds.center.x, 0));
            }
        }
    }
}
