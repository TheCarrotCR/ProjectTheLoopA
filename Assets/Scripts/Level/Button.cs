using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public int pressed;
    public Door door1;
    public Door door2;
    public Door door3;
    public Door door4;
    public Door door5;

    private Door[] doors;

    void Start()
    {
      doors = new[] { door1, door2, door3, door4, door5 };
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
      if (other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom")
        pressed++;
      if (pressed > 0)
      {
        var m_Animator = GetComponent<Animator>();
        m_Animator.ResetTrigger("Off");
        m_Animator.SetTrigger("On");
      }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
      if (other.gameObject.tag == "Player" || other.gameObject.tag == "Phantom")
        pressed--;
      if (pressed <= 0)
      {
        var m_Animator = GetComponent<Animator>();
        m_Animator.ResetTrigger("On");
        m_Animator.SetTrigger("Off");
      }
    }

    void FixedUpdate()
    {
      foreach (var door in doors)
        if (door != null)
          door.isOpened = pressed > 0;
    }
    
}
