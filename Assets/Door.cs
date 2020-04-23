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
      GetComponent<BoxCollider2D>(). enabled = !isOpened;
    }
}
