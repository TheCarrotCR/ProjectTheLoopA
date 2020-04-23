using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
  public bool isOpened;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<BoxCollider2D>().enabled = !isOpened;
        var m_Animator = GetComponent<Animator>();
        if (isOpened)
        {
          m_Animator.ResetTrigger("Closing");
          m_Animator.SetTrigger("Open");
        }
        else
        {
          m_Animator.ResetTrigger("Open");
          m_Animator.SetTrigger("Closing");
        }
    }
}
