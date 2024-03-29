using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private readonly float movementForce = 10f;
    private readonly float airMovementForce = 4f;
    private readonly float jumpVelocity = 140f;
    private readonly float jumpForceAir = 7f;
    public Rigidbody2D rb;
    private Vector2 movementDirection;
    private bool touchingGround;
    private bool jumping;
    private bool ignorePlatform;
    private float idleTime;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip lorenzoMeow1;
    public AudioClip lorenzoMeow2;
    public AudioClip lorenzoPurring;
    private float timeSinceMeow;
    private bool playingMeow;

    // Start is called before the first frame update
    void Start()
    {
        touchingGround = false;
        jumping = false;
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(6,1, true);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playingMeow = false;
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
        if (Input.GetAxis("Vertical") > 0) jumping = true;
        else jumping = false;

        if (rb.velocity.x < 0) GetComponent<SpriteRenderer>().flipX = true;
        else if (rb.velocity.x > 0) GetComponent<SpriteRenderer>().flipX = false;
        animator.SetFloat("Speed", Math.Abs(rb.velocity.x));
        animator.SetBool("OnGround", touchingGround);
        animator.SetFloat("IdleTime", idleTime);
    }

    void FixedUpdate() 
    {
        Vector2 force = Vector2.zero;
        
        if (touchingGround) 
        {
            force += movementDirection * movementForce - rb.velocity;
            if (jumping) force += Vector2.up * jumpVelocity;
        }
        else force += movementDirection * airMovementForce - rb.velocity;
        rb.AddForce(force);
        if (jumping && rb.velocity.y > 0) rb.AddForce(Vector2.up * jumpForceAir);
        
        ignorePlatform = rb.velocity.y > 0 || Input.GetAxis("Vertical") < 0; 
        Physics2D.IgnoreLayerCollision(6, 7, ignorePlatform);

        idleTime += Time.deltaTime;
        if (math.abs(rb.velocity.x) > 0 || math.abs(rb.velocity.y) > 0) idleTime = 0;

        timeSinceMeow += Time.deltaTime;
        if (timeSinceMeow > 10 && !playingMeow) {
            int meowToPlay = UnityEngine.Random.Range(0,2);
            if (meowToPlay == 0) audioSource.PlayOneShot(lorenzoMeow1);
            else audioSource.PlayOneShot(lorenzoMeow2);

            if (idleTime > 10 ) audioSource.PlayOneShot(lorenzoPurring);
            playingMeow = true;
            timeSinceMeow = UnityEngine.Random.Range(-5, 0);
        }
        else playingMeow = false;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 normal = other.GetContact(0).normal;
        if (other.gameObject.CompareTag("Ground") && normal == Vector2.up) {
            touchingGround = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Vector2 normal = other.GetContact(0).normal;
        if (other.gameObject.CompareTag("Ground") && normal == Vector2.up) {
            touchingGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Ground")) touchingGround = false;
    }
}
