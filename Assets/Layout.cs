using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout : MonoBehaviour
{
    public KeyCode moveUp1;
    public KeyCode moveUp2;
    public KeyCode moveDown1;
    public KeyCode moveDown2;
    public KeyCode moveLeft1;
    public KeyCode moveLeft2;
    public KeyCode moveRight1;
    public KeyCode moveRight2;

    public bool PressedMoveRight => Input.GetKey(moveRight1) || Input.GetKey(moveRight2); 
    public bool PressedMoveLeft => Input.GetKey(moveLeft1) || Input.GetKey(moveLeft2); 
    public bool PressedMoveUp => Input.GetKey(moveUp1) || Input.GetKey(moveUp2); 
    public bool PressedMoveDown => Input.GetKey(moveDown1) || Input.GetKey(moveDown2); 
}
