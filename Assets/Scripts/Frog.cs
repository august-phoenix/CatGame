using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy {
    private Collider2D coll;

    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] LayerMask ground;


    private bool facingLeft = true;

    protected override void Start() {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Update() {
        // Transition from jump to fall
        if (anim.GetBool("Jumping")) {
            if (rb.velocity.y < .1f) {
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", true);
            }
        }

        // Transition from fall to idle
        if (coll.IsTouchingLayers(ground) && anim.GetBool("Falling")) { 
            anim.SetBool("Falling", false);
        }
    }

    private void Move() {
        
        if (facingLeft) {

            if (transform.position.x > leftCap) {
                if (coll.IsTouchingLayers(ground)) {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);

                    if (transform.localScale.x != 1) {
                        transform.localScale = new Vector3(1, 1);
                    }
                }
            }
            else {
                facingLeft = false;
            }
        }


        else {
            if (transform.position.x < rightCap) {
                

                if (coll.IsTouchingLayers(ground)) {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                    
                    if (transform.localScale.x != -1) {
                        transform.localScale = new Vector3(-1, 1);
                    }
                }
            }
            else {
                facingLeft = true;
            }
        }
    }

    

}


                                                       