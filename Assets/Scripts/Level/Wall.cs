using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom")
        {
            var distance = other.collider.Distance(other.otherCollider);
            other.transform.Translate(distance.normal * distance.distance - distance.normal * 0.0001f);
        }
    }
}
