using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
  private BoxCollider2D curCollider;
  private Animator curAnimator;
  public bool isOpened;

    void Start()
    {
        curCollider = GetComponent<BoxCollider2D>();
        curAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        curCollider.enabled = !isOpened;
        if (isOpened)
        {
          curAnimator.ResetTrigger("Closing");
          curAnimator.SetTrigger("Open");
        }
        else
        {
          curAnimator.ResetTrigger("Open");
          curAnimator.SetTrigger("Closing");
        }
    }
}
