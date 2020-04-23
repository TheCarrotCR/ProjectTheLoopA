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
<<<<<<< HEAD:Assets/Door.cs
      GetComponent<BoxCollider2D>(). enabled = !isOpened;
=======
        GetComponent<BoxCollider2D>().enabled = !isOpened;
>>>>>>> c0b70b1b02625e70f314e860ba2d920431634671:Assets/Scripts/Level/Door.cs
    }
}
