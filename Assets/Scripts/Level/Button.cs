using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool pressed;
    public Door door1;

    private void OnTriggerStay2D(Collider2D other) 
    {
      pressed = other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom"; 
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
      pressed = !(other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom");
    }

    void Start()
    {
        
    }

    void Update()
    {
      door1.isOpened = pressed;
    }
    
}
