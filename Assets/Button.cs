using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool pressed;
    public Door door1;

     private void OnTriggerEnter2D(Collider2D other) 
     {
        enabled = other.gameObject.tag == "Player";
     }
    void Start()
    {
        
    }

    void Update()
    {
      door1.isOpened = pressed;
    }
    
}
