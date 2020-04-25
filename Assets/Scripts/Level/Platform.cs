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

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom")
        {
            if (other.gameObject.GetComponent<Player>().CanClimb())
                return;
            var distance = other.collider.Distance(other.otherCollider);
            other.transform.Translate(distance.normal * distance.distance  - distance.normal * 0.0001f);
        }
    }
}
