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
      if (pressed)
      {
        var m_Animator = GetComponent<Animator>();
        m_Animator.ResetTrigger("Off");
        m_Animator.SetTrigger("On");
      }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
      pressed = !(other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom");
      if (!pressed)
      {
        var m_Animator = GetComponent<Animator>();
        m_Animator.ResetTrigger("On");
        m_Animator.SetTrigger("Off");
      }
    }

    void FixedUpdate()
    {
      door1.isOpened = pressed;
    }
    
}
