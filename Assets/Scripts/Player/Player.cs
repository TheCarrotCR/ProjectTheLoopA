using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Layout controls;
    private Vector2 prevPosition;
    private bool allowedMoveLeft;
    private bool allowedMoveRight;
    private bool allowedClimbing;

    public enum State { Idle, Climb, Run }

    public Phantom phantom;
    public bool isPhantom;
    public GameObject playerPhantom;
    public State state;
    public Vector2 speedVector;
    public Vector2 speedMax;
    public Vector2 speedStep;

    void Start()
    {
        controls = GameObject.FindWithTag("Controls").GetComponent<Layout>();
        allowedMoveLeft = true;
        allowedMoveRight = true;
        allowedClimbing = false;
        prevPosition = GetComponent<Rigidbody2D>().position;
        gameObject.SetActive(!isPhantom);
    }

    void FixedUpdate()
    {
        if (isPhantom)
        {
            phantom.Play(this);
            return;
        }
        if (!controls.PressedTimeback) 
        {
            PerformMovement();
            phantom.Remember(state, transform.localScale, GetComponent<Rigidbody2D>().position - prevPosition, allowedMoveLeft, allowedMoveRight, allowedClimbing);
        }
        else
        {
            var memento = phantom.Pop();
            if (!memento.isNull)
            {
                SetMovementPermissions(memento.allowedMoveLeft, memento.allowedMoveRight, memento.allowedClimbing);
                speedVector = memento.speedVector;
                transform.localScale = memento.transformScale;
                state = memento.state;
                GetComponent<Rigidbody2D>().gravityScale = speedVector.y != 0 ? 0 : 1;
                transform.Translate(new Vector2(-speedVector.x, -speedVector.y));
            }
        }
        prevPosition = GetComponent<Rigidbody2D>().position;
        if (controls.PressedPlayPhantom && playerPhantom != null)
        {
            if (playerPhantom.activeSelf) return;
            playerPhantom.GetComponent<Player>().phantom.memory = new List<Phantom.MemoryCell>(this.phantom.memory);
            playerPhantom.SetActive(true);
        }
    }

    public void SetMovementPermissions(bool allowedMoveLeft, bool allowedMoveRight, bool allowedClimbing)
    {
        this.allowedMoveLeft = allowedMoveLeft;
        this.allowedMoveRight = allowedMoveRight;
        this.allowedClimbing = allowedClimbing;
    }

    public bool CanClimb() => allowedClimbing;
    public bool CanMoveLeft() => allowedMoveLeft;
    public bool CanMoveRight() => allowedMoveRight;

    void PerformMovement() 
    {
        PerformClimb();
        GetComponent<Rigidbody2D>().gravityScale = state == State.Climb ? 0 : 1;
        if (state != State.Climb)
        {
            speedVector.y = 0;
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

    void PerformClimb()
    {
        if (allowedClimbing) 
        {
            if (controls.PressedMoveUp || controls.PressedMoveDown)
                speedVector.y = controls.PressedMoveUp ? speedMax.y : -speedMax.y;
            else 
            {
                speedVector.y = 0;
                if (state == State.Climb)
                    state = State.Idle;
                return;
            }
            state = State.Climb;
        }
        else if (state == State.Climb)
            state = State.Idle;
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
            allowedClimbing = true;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.GetComponent<ClimbyThing>() != null)
            allowedClimbing = false;
    }
}
