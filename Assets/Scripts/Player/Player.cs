using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D curRigidbody;
    private Animator curAnimator;
    private Layout controls;
    private Vector3 prevPosition;
    private bool allowedMoveLeft;
    private bool allowedMoveRight;
    private bool allowedClimbing;
    private bool allowedClimbingUp;
    private bool allowedClimbingDown;

    public enum State { Idle, Climb, Run }

    public Phantom phantom;
    public bool isPhantom;
    public GameObject playerPhantom;
    public State state;
    public bool inAir;
    public Vector2 speedVector;
    public Vector2 speedMax;
    public Vector2 speedStep;

    void Start()
    {
        curRigidbody = GetComponent<Rigidbody2D>();
        curAnimator = GetComponent<Animator>();
        controls = GameObject.FindWithTag("Controls").GetComponent<Layout>();

        allowedMoveLeft = true;
        allowedMoveRight = true;
        allowedClimbing = false;
        allowedClimbingUp = true;
        allowedClimbingDown = true;
        inAir = true;
        prevPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (isPhantom)
        {
            phantom.Play(this);
            TriggerAnimations();
            return;
        }
        if (!controls.PressedTimeback)
        {
            PerformMovement();
            var vToRemember = transform.position - prevPosition;
            phantom.Remember(state, transform.localScale, new Vector2(vToRemember.x, vToRemember.y), allowedMoveLeft, allowedMoveRight, allowedClimbing);
            prevPosition = transform.position;
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
                transform.Translate(new Vector2(-speedVector.x, -speedVector.y));
            }
        }
        if (controls.PressedPlayPhantom && playerPhantom != null)
        {
            if (playerPhantom.activeSelf) return;
            playerPhantom.GetComponent<Phantom>().memory = new List<Phantom.MemoryCell>(this.phantom.memory);
            playerPhantom.SetActive(true);
        }
        TriggerAnimations();
    }

    void TriggerAnimations()
    {
        switch (state) {
            case State.Climb:
                curAnimator.ResetTrigger("Stop");
                curAnimator.ResetTrigger("Run");
                curAnimator.SetTrigger("Climb");
                break;
            case State.Run:
                curAnimator.ResetTrigger("Stop");
                curAnimator.ResetTrigger("Climb");
                curAnimator.SetTrigger("Run");
                break;
            default:
                curAnimator.ResetTrigger("Run");
                curAnimator.ResetTrigger("Climb");
                curAnimator.SetTrigger("Stop");
                break;
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
        if (state != State.Climb)
        {
            speedVector.y = inAir ? -speedMax.y : 0;
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
        if (!allowedMoveLeft && speedVector.x < 0 || !allowedMoveRight && speedVector.x > 0)
            speedVector.x = 0;
    }

    void PerformClimb()
    {
        if (allowedClimbing) 
        {
            if (controls.PressedMoveUp && allowedClimbingUp || controls.PressedMoveDown && allowedClimbingDown)
                speedVector.y = controls.PressedMoveUp ? speedMax.y : -speedMax.y;
            else 
            {
                speedVector.y = 0;
                return;
            }
            state = State.Climb;
        }
        else if (state == State.Climb)
        {
            state = State.Idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var bounds = other.otherCollider.bounds;
        var otherBounds = other.collider.bounds;
        if (other.gameObject.tag == "Wall") 
        {
            allowedMoveLeft = otherBounds.center.x > bounds.center.x;
            allowedMoveRight = otherBounds.center.x < bounds.center.x;
            if (!allowedMoveLeft && !allowedMoveRight && state != State.Climb)
            {
                allowedMoveLeft = true;
                allowedMoveRight = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        var bounds = other.otherCollider.bounds;
        var otherBounds = other.collider.bounds;
        if (other.gameObject.tag == "Platform")
        {
            inAir = false;
            if (!other.gameObject.GetComponent<Platform>().passable)
            {
                state = State.Idle;
                speedVector.y = 0;
                if (otherBounds.center.y < bounds.center.y)
                    allowedClimbingDown = false;
                if (otherBounds.center.y > bounds.center.y)
                    allowedClimbingUp = false;
            }
            else
            {
                if (otherBounds.center.y < bounds.center.y)
                    allowedClimbingDown = true;
                if (otherBounds.center.y > bounds.center.y)
                    allowedClimbingUp = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Wall") 
        {
            allowedMoveLeft = true;
            allowedMoveRight = true;
        }
        if (other.gameObject.tag == "Platform")
            if (state != State.Climb) 
                inAir = true;
            else
            {
                allowedClimbingDown = true;
                allowedClimbingUp = true;
            }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.GetComponent<ClimbyThing>() != null)
            allowedClimbing = true;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.GetComponent<ClimbyThing>() != null)
        {
            if (state == State.Climb)
            {
                inAir = true;
                state = State.Idle;
            }
            allowedClimbing = false;
        }
    }
}
