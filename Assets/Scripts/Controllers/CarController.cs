using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float steering;
    [SerializeField]
    private float friction;
    [SerializeField]
    private float angularFriction;
    [SerializeField]
    private LayerMask ObservationIgnore;
    [SerializeField]
    private LayerMask LocatorIgnore;

    public float[] outputs = new float[4];
    public int distance = 0;
    public float time = 0;
    private float startTime;

    private Vector3 previousPosition;
    private float previousCheckTime = 0;
    private float stopTime = 3f;

    private Rigidbody2D rb;

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
        this.rb = GetComponentInChildren<Rigidbody2D>();
        startTime = Time.time;
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Observe surroundings
        outputs[0] = Observe(0);
        outputs[1] = Observe(30);
        outputs[2] = Observe(180);
        outputs[3] = Observe(330);

        // Get input
        float[] control = RandomControl();

        // Move vehicle
        Move(control[0], control[1]);
        NoCrashing();

        // Track time
        time = Time.time - startTime;

    }

    // Random control
    private float[] RandomControl()
    {
        float h = UnityEngine.Random.Range(-1f, 1f);
        float v = UnityEngine.Random.Range(0f, 1f);

        float[] output = { h, v };

        return output;
    }


    // Movement
    private void Move(float h, float v)
    {
        // Calculate speed from input and acceleration (transform.up is forward)
        Vector2 speed = transform.up * (v * acceleration);
        rb.AddForce(speed);

        // Create car rotation
        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f)
        {
            rb.rotation += h * steering * (rb.velocity.magnitude / maxSpeed);
        }
        else
        {
            rb.rotation -= h * steering * (rb.velocity.magnitude / maxSpeed);
        }

        // Change velocity based on rotation
        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.left)) * 2.0f;
        Vector2 relativeForce = Vector2.right * driftForce;
        //Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green);
        rb.AddForce(rb.GetRelativeVector(relativeForce));

        // Force max speed limit
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Friction
        if (v == 0)
        {
            rb.velocity = rb.velocity * friction;
        }
        if (h == 0)
        {
            rb.angularVelocity = rb.angularVelocity * angularFriction;
        }
    }

    // Observation of surroundings
    private float Observe(float angle)
    {
        // Start point of ray
        Vector3 raySource = transform.position;

        // Angle of the ray
        Quaternion eulerAngle = Quaternion.Euler(0f, 0f, angle);
        Vector3 rayDirection = eulerAngle * transform.up;

        // Raycasting
        RaycastHit2D hit = Physics2D.Raycast(raySource, rayDirection, 5f, ~ObservationIgnore);
        if (hit)
        {
            Debug.DrawLine(raySource, hit.point, Color.red);
        }

        return hit.distance >= 0 ? hit.distance : -1f;
    }

    // Iterate Distance & Destroy if goes backwards
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "checkpoint")
        {
            float dot = Vector3.Dot(transform.up, collision.transform.right);
            if (dot < 0)
            {
                this.gameObject.SetActive(false);
            }
            else { distance += 1; }
        }
    }

    // Destroy if stopped for too long
    private void NoCrashing()
    {
        if ((Time.time - previousCheckTime) > stopTime)
        {
            if ((transform.position - previousPosition).magnitude < 0.1f)
            {
                this.gameObject.SetActive(false);
            }
            previousPosition = transform.position;
            previousCheckTime = Time.time;
        }
    }
}
