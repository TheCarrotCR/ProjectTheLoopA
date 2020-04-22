using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //private Rigidbody2D rigidbody;
    private Layout controls;
    private bool allowedMoveLeft;
    private bool allowedMoveRight;
    private bool allowedClimbingUp;
    private bool allowedClimbingDown;

    public enum State {
        Idle,
        IdleClimb,
        Climb,
        Run
    }

    public State state;
    public Vector2 speedVector;
    public Vector2 speedMax;
    public Vector2 speedStep;

    void Start()
    {
        //rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        controls = GameObject.FindWithTag("Controls").GetComponent<Layout>();
        allowedMoveLeft = true;
        allowedMoveRight = true;
        allowedClimbingUp = false;
        allowedClimbingDown = false;
    }

    void FixedUpdate()
    {
        PerformMovement();
    }

    void PerformMovement() 
    {
        var currentRigidbody = GetComponent<Rigidbody2D>();
        PrepareClimb();
        if (state == State.Climb || state == State.IdleClimb)
        {
            var currentVelocity = currentRigidbody.velocity;
            currentVelocity.y = 0;
            currentRigidbody.velocity = currentVelocity;
            currentRigidbody.gravityScale = 0;
            PerformClimb();
        }
        else
        {
            speedVector.y = 0;
            currentRigidbody.gravityScale = 1;
            PrepareMovement();
            if (speedVector.x != 0) 
            {
                var scale = transform.localScale;
                scale.x = Mathf.Abs(transform.localScale.x) * speedVector.x / Mathf.Abs(speedVector.x);
                transform.localScale = scale;
                state = State.Run;
            }
            else
                state = State.Idle;
        }
        transform.Translate(speedVector);
    }

    void PrepareMovement()
    {
        if (!(allowedMoveLeft && allowedMoveRight || speedVector.x == 0))
            speedVector.x = 0;
        var direction = speedVector.x / Mathf.Abs(speedVector.x);
        if (float.IsNaN(direction))
            direction = controls.PressedMoveLeft ? -1 : 1;
        speedVector.x = Mathf.Abs(speedVector.x);
        if (allowedMoveRight && controls.PressedMoveRight && direction > 0 
            || allowedMoveLeft && controls.PressedMoveLeft && direction < 0)
            speedVector.x += speedStep.x;
        else if (speedVector.x > 0)
            speedVector.x -= speedStep.x * 2.5f;
        if (speedVector.x < 0 || speedVector.x > speedMax.x)
            speedVector.x = speedVector.x < 0 ? 0 : speedMax.x;
        speedVector.x *= direction;
    }

    void PrepareClimb()
    {
        if (allowedClimbingDown && controls.PressedMoveDown 
            || allowedClimbingUp && controls.PressedMoveUp)
        {
            state = State.Climb;
            return;
        }
        if (state == State.Climb)
            state = State.IdleClimb;
    }

    void PerformClimb()
    {
        speedVector.x = 0;
        var direction = speedVector.y / Mathf.Abs(speedVector.y);
        if (float.IsNaN(direction))
            direction = controls.PressedMoveDown ? -1 : 1;
        speedVector.y = Mathf.Abs(speedVector.y);
        if (controls.PressedMoveUp && direction > 0 
            || controls.PressedMoveDown && direction < 0)
            speedVector.y += speedStep.y;
        else if (speedVector.y > 0)
            speedVector.y -= speedStep.y * 2.5f;
        if (speedVector.y < 0 || speedVector.y > speedMax.y)
            speedVector.y = speedVector.y < 0 ? 0 : speedMax.y;
        speedVector.y *= direction;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        var bounds = other.otherCollider.bounds;
        var otherBounds = other.collider.bounds;
        if (other.gameObject.GetComponent<Wall>() != null) 
        {
            allowedMoveLeft = otherBounds.center.x > bounds.center.x;
            allowedMoveRight = otherBounds.center.x < bounds.center.x;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.GetComponent<Wall>() != null) 
        {
            allowedMoveLeft = true;
            allowedMoveRight = true;
        }       
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (state != State.Climb && other.gameObject.GetComponent<ClimbyThing>() != null)        
        {
            allowedClimbingUp = true;
            allowedClimbingDown = true;
        }  
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.GetComponent<ClimbyThing>() != null)        
        {
            allowedClimbingUp = false;
            allowedClimbingDown = false;
            state = speedVector.x == 0 ? State.Idle : State.Run;
        }  
    }
}
