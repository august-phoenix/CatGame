using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatController : MonoBehaviour {
    //Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    //FSM
    private enum State { idle, running, jumping, falling, hurt, walking }
    private State state = State.idle;

    //Inspector variables
    [SerializeField] private LayerMask path;
    [SerializeField] private LayerMask grass;
    [SerializeField] private LayerMask wood;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private AudioSource jumping_path;
    [SerializeField] private AudioSource falling_path;
    [SerializeField] private AudioSource walking_path;
    [SerializeField] private AudioSource jumping_grass;
    [SerializeField] private AudioSource falling_grass;
    [SerializeField] private AudioSource walking_grass;
    [SerializeField] private AudioSource jumping_wood;
    [SerializeField] private AudioSource falling_wood;
    [SerializeField] private AudioSource walking_wood;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {

        if (state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); //sets animation based on Enumerator state
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        //Moving left

        if (hDirection < 0)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                rb.velocity = new Vector2(-runningSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-walkingSpeed, rb.velocity.y);
            }
            transform.localScale = new Vector2(-1, 1);
        }

        //Moving right

        if (hDirection > 0) {
            if (Input.GetButtonDown("Sprint"))
            {
                rb.velocity = new Vector2(runningSpeed, rb.velocity.y);
            }
            else {
                rb.velocity = new Vector2(walkingSpeed, rb.velocity.y);
            }
            transform.localScale = new Vector2(1, 1);
        }

        //Jumping

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(path))
        {
            jumping_path.Play();
            jump();
        }
        else if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(grass))
        {
            jumping_grass.Play();
            jump();
        }
        else if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(wood))
        {
            jumping_wood.Play();
            jump();
        }
    }
    private void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimationState()
    {

        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }

        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(path))
            {
                falling_path.Play();
                state = State.idle;
            }
            else if (coll.IsTouchingLayers(grass))
            {
                falling_grass.Play();
                state = State.idle;
            }
            else if (coll.IsTouchingLayers(wood))
            {
                falling_wood.Play();
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > walkingSpeed)
        {
            //Moving
            state = State.running;
        }

        else if (Mathf.Abs(rb.velocity.x) > 0 && Mathf.Abs(rb.velocity.x) < runningSpeed)
        {
            state = State.walking;
        }
        else
        {
            state = State.idle;
        }
    }

    private void Footstep()
    {
        if (coll.IsTouchingLayers(path))
        {
            walking_path.Play();
        }
        else if (coll.IsTouchingLayers(grass))
        {
            walking_grass.Play();
        }
        else if (coll.IsTouchingLayers(wood))
        {
            walking_wood.Play();
        }
    }
}
