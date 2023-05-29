using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BabyCatAI : MonoBehaviour
{
    private enum State { idle, running, jumping, falling, hurt, walking }
    private State state = State.idle;

    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = .5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirements = .8f;
    public float jumpModifier = .3f;
    public float jumpCheckOffset = .1f;

    [Header("Custum Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;
    Animator anim;

    public void Start() {
        seeker = GetComponent<Seeker>();
        rb     = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate() {
        if (TargetInDistance() && followEnabled) {
            PathFollow();
            anim.SetInteger("state", (int)state); //sets animation based on Enumerator state
            Running();
        }
    }

    private void UpdatePath() {
        if (followEnabled && TargetInDistance() && seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow() {
        Running();
        if (path == null) {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            return;
        }

        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force     = direction * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded) {
            if (direction.y > jumpNodeHeightRequirements) {
                rb.AddForce(Vector2.up * speed * jumpModifier);
                state = State.jumping;
                if (rb.velocity.y < .1f)
                {
                    state = State.falling;
                }
            }
        }

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        if (directionLookEnabled) {
            if (rb.velocity.x > 0.05f) {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            else if (rb.velocity.x < -0.05f) {
                transform.localScale = new Vector3( -1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance() {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0; 
        }
    }
    private void Running() {
        if (Mathf.Abs(rb.velocity.x) > 1f)
        {
            //Moving
            state = State.running;
        }
        else if (Mathf.Abs(rb.velocity.x) < 1f) {
            state = State.idle;
        }
    }
}

