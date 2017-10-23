using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Player : MonoBehaviour {

    public Camera mainCamera;
    public float maximumSpeed = 5.5f;
    public float acceleration = 5f;
    public float jumpingForce = 100f;
    public float jumpingCooldown = 0.6f;
    public bool reachedFinishLine = false;
    public float turnaroundDuration = 10f;

    private float speed = 0f;
    private float jumpingTimer = 0f;
    private float progress;
    private bool inverseDirection;

    private BezierSpline path;
    // Use this for initialization
    void Start() {}

    // Update is called once per frame
    void Update() {
        // move the player forward
        if (!reachedFinishLine && path == null) {
            speed += acceleration * Time.deltaTime;
            if (speed > maximumSpeed) {
                speed = maximumSpeed;
            }
        }

        if (path == null) {
            transform.position += (inverseDirection ? -speed : speed) * Vector3.forward * Time.deltaTime;
        } else {
            // running on the spline
            progress += Time.deltaTime / turnaroundDuration;
            if (progress > 1f) {
                progress = 0;
                inverseDirection = !inverseDirection;
                path = null;
                return;
            }

            Vector3 pathPosition = path.GetPoint(progress);
            transform.localPosition = pathPosition;
            transform.LookAt(pathPosition + path.GetDirection(progress));
        }
        // make the player jump
        jumpingTimer -= Time.deltaTime;
        if (!reachedFinishLine && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))) {
            if (jumpingTimer <= 0f) {
                jumpingTimer = jumpingCooldown;
                GetComponent<Rigidbody>().AddForce(Vector3.up * jumpingForce);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("hit something: " + other.tag);
        if (other.tag.Equals("Obstacle")) {
            Decelerate(speed * 0.5f);
        }

        if (other.tag.Equals("FinishLine")) {
            reachedFinishLine = true;
        }

        if (other.tag.Equals("TurnAround")) {
            path = other.GetComponent<TurnaroundTrigger>().path;
        }
    }

    public void Decelerate(float speedValue) {
        speed -= speedValue;
        if (speed < 0f) {
            speed = 0;
        }
    }
}