using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Camera mainCamera;
    public float maximumSpeed = 5.5f;
    public float acceleration = 5f;
    public float jumpingForce = 100f;
    public float jumpingCooldown = 0.6f;
    public bool reachedFinishLine = false;

    private float speed = 0f;
    private float jumpingTimer = 0f;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // move the player forward
        speed += acceleration * Time.deltaTime;
        if (speed > maximumSpeed) {
            speed = maximumSpeed;
        }

        transform.position += maximumSpeed * Vector3.forward * Time.deltaTime;
        // make the player jump
        jumpingTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) {
            if (jumpingTimer <= 0f) {
                jumpingTimer = jumpingCooldown;
                GetComponent<Rigidbody>().AddForce(Vector3.up * jumpingForce);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("hit something: " + other.tag);
        if (other.tag.Equals("Obstacle")) {
            speed *= 0.5f;
        }

        if (other.tag.Equals("FinishLine")) {
            reachedFinishLine = true;
        }
    }
}